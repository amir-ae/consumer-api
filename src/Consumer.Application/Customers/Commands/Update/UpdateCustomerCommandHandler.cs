using Consumer.API.Contract.V1.Customers.Messages;
using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Application.Products.Commands.Create;
using Consumer.Application.Products.Commands.Update;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Update;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderingService _orderingService;
    private readonly ISender _mediator;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IOrderingService orderingService, 
        ISender mediator)
    {
        _customerRepository = customerRepository;
        _orderingService = orderingService;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Customer>> Handle(UpdateCustomerCommand command, CancellationToken ct = default)
    {
        var (customerId, firstName, middleName, lastName, phoneNumber, 
            cityId, address, role, products, orders, 
            updateBy, updateAt, onCreate, version) = command;
        
        var customer = await _customerRepository.ByIdAsync(customerId, ct);
        if (customer is null) return Error.NotFound(
            nameof(CustomerId), $"{nameof(Customer)} with id {customerId} is not found.");

        Action<CustomerEvent, int?> append = _customerRepository.Append;
        CustomerUpdateMessage? updateMessage = null;

        customer = customer.UpdateName(firstName, middleName, lastName, updateBy, updateAt, append, ref version, 
            out bool nameUpdated);
        
        if (nameUpdated)
        {
            updateMessage = new CustomerUpdateMessage(customerId.Value, customer.FullName, updateBy.Value, updateAt);
        }

        customer = customer.UpdatePhoneNumber(phoneNumber, updateBy, updateAt, append, ref version);

        customer = customer.UpdateAddress(cityId, address, updateBy, updateAt, append, ref version);

        customer = customer.AddOrders(orders, updateBy, updateAt, append, ref version);

        if (!onCreate)
        {
            customer = customer.RemoveOrders(orders, updateBy, updateAt, append, ref version);
        }
        
        bool roleChanged = false;
        customer = customer.UpdateRole(role, updateBy, updateAt, append, ref version, out bool roleUpdated);
        if (roleUpdated)
        {
            roleChanged = true;
            foreach (var productId in customer.ProductIds.ToList())
            {
                var updateProductCommand = new UpdateProductCommand(
                    productId, null, null, 
                    customer.Role is CustomerRole.Owner ? new UpsertCustomerCommand(customerId) : new UpsertCustomerCommand(new CustomerId(string.Empty)), 
                    customer.Role is CustomerRole.Dealer ? new UpsertCustomerCommand(customerId) : new UpsertCustomerCommand(new CustomerId(string.Empty)),
                    null, null, null, null, null, 
                    null, null, null, null, 
                    null, null, updateBy, updateAt, false, true);

                await _mediator.Send(updateProductCommand, ct);
            }
        }
        
        var productIds = products?.Select(p => p.ProductId).ToHashSet();
        var productOwnerId = customer.Role is CustomerRole.Owner ? new UpsertCustomerCommand(customerId) : null;
        var productDealerId = customer.Role is CustomerRole.Dealer ? new UpsertCustomerCommand(customerId) : null;

        if (productIds is not null)
        {
            if (!productIds.SetEquals(customer.ProductIds))
            {
                var productIdsToAdd = productIds.Except(customer.ProductIds).ToList();
                var productsToAdd = products?.Where(p => productIdsToAdd.Contains(p.ProductId)).ToList();
            
                foreach (var p in productsToAdd ?? new())
                {
                    if (!p.IsId)
                    {
                        var createProduct = new CreateProductCommand(
                            p.ProductId, p.Brand!, p.Model!, p.SerialId, productOwnerId, productDealerId,
                            p.DeviceType, p.PanelModel, p.PanelSerialNumber, p.WarrantyCardNumber, p.DateOfPurchase, 
                            p.InvoiceNumber, p.PurchasePrice, p.Orders, p.IsUnrepairable, 
                            p.DateOfDemandForCompensation, p.DemanderFullName, updateBy, updateAt);

                        await _mediator.Send(createProduct, ct);
                    }
                    else
                    {
                        var updateProduct = new UpdateProductCommand(
                            p.ProductId, null, null, productOwnerId, productDealerId,
                            null, null, null, null, null, 
                            null, null, null, null, 
                            null, null, updateBy, updateAt);

                        await _mediator.Send(updateProduct, ct);
                    }
                }
            }

            if (!onCreate || roleChanged)
            {
                var productsToUpdate = products?.Where(p => customer.ProductIds.Contains(p.ProductId)).ToList();
            
                var updatedProductOwnerId = productOwnerId;
                var updatedProductDealerId = productDealerId;

                if (roleChanged)
                {
                    updatedProductOwnerId ??= new UpsertCustomerCommand(new CustomerId(string.Empty));
                    updatedProductDealerId ??= new UpsertCustomerCommand(new CustomerId(string.Empty));
                }

                foreach (var p in productsToUpdate ?? new())
                {
                    var updateProduct = new UpdateProductCommand(
                        p.ProductId, p.Brand, p.Model, updatedProductOwnerId, updatedProductDealerId,
                        p.DeviceType, p.PanelModel, p.PanelSerialNumber, p.WarrantyCardNumber, p.DateOfPurchase, 
                        p.InvoiceNumber, p.PurchasePrice, p.Orders, p.IsUnrepairable, 
                        p.DateOfDemandForCompensation, p.DemanderFullName, updateBy, updateAt, false, roleChanged);

                    await _mediator.Send(updateProduct, ct);
                }
            }
            
            if (!onCreate)
            {
                var productIdsToRemove = customer.ProductIds.Except(productIds);
                foreach (var productId in productIdsToRemove)
                {
                    var updateProduct = new UpdateProductCommand(
                        productId, null, null, productOwnerId, productDealerId,
                        null, null, null, null, null, 
                        null, null, null, null, 
                        null, null, updateBy, updateAt);

                    await _mediator.Send(updateProduct, ct);
                }
            }
        }

        if (updateMessage is not null)
        {
            await _orderingService.PublishCustomerUpdateAsync(updateMessage, ct);
        }
        
        await _customerRepository.SaveChangesAsync(ct);

        return customer;
    }
}