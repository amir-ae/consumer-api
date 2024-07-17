using Consumer.API.Contract.V1.Customers.Messages;
using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Application.Products.Commands.Create;
using Consumer.Application.Products.Commands.Update;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Consumer.Application.Customers.Commands.Update;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ErrorOr<Customer>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ILookupService _lookupService;
    private readonly ISender _mediator;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork, IEventSubscriptionManager subscriptionManager, 
        ILookupService lookupService, ISender mediator
    )
    {
        _unitOfWork = unitOfWork;
        _customerRepository = unitOfWork.CustomerRepository;
        _subscriptionManager = subscriptionManager;
        _lookupService = lookupService;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Customer>> Handle(UpdateCustomerCommand command, CancellationToken ct = default)
    {
        var (customerId, firstName, middleName, lastName, phoneNumber, 
            cityId, address, role, products, orders, 
            updateBy, updateAt, saveChanges, isOnCreate, version) = command;
        
        var customer = await _customerRepository.ByIdAsync(customerId, ct);
        if (customer is null) return Error.NotFound(
            nameof(CustomerId), $"{nameof(Customer)} with id {customerId} is not found.");
        
        if (version.HasValue && version != customer.Version) return Error.Conflict(
            nameof(StatusCodes.Status412PreconditionFailed), $"{nameof(customer.Version)} mismatch.");

        _subscriptionManager.SubscribeToCustomerEvents(customer);
        CustomerUpdateMessage? updateMessage = null;

        customer.UpdateName(firstName, middleName, lastName, updateBy, updateAt, out bool nameUpdated);
        
        if (nameUpdated)
        {
            updateMessage = new CustomerUpdateMessage(customerId.Value, customer.FullName, updateBy.Value, updateAt);
        }

        phoneNumber = await _lookupService.InspectCountryCode(phoneNumber, ct);
        
        customer.UpdatePhoneNumber(phoneNumber, updateBy, updateAt);

        customer.UpdateAddress(cityId, address, updateBy, updateAt);

        customer.AddOrders(orders, updateBy, updateAt);

        if (!isOnCreate)
        {
            customer.RemoveOrders(orders, updateBy, updateAt);
        }
        
        var productIds = products?.Select(p => p.ProductId).ToHashSet();
        var customerProductIds = customer.ProductIds;
        var customerRole = customer.Role;

        customer.UpdateRole(role, updateBy, updateAt, out bool roleUpdated);

        var productOwnerId = customer.Role == CustomerRole.Owner ? new UpsertCustomerCommand(customerId) : null;
        var productDealerId = customer.Role == CustomerRole.Dealer ? new UpsertCustomerCommand(customerId) : null;
        
        if (productIds is not null && saveChanges)
        {
            if (!productIds.SetEquals(customerProductIds))
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
                            p.DateOfDemandForCompensation, p.DemanderFullName, updateBy, updateAt, false);

                        await _mediator.Send(createProduct, ct);
                    }
                    else
                    {
                        var updateProduct = new UpdateProductCommand(
                            p.ProductId, null, null, productOwnerId, productDealerId,
                            null, null, null, null, null, 
                            null, null, null, null, 
                            null, null, updateBy, updateAt, false);

                        await _mediator.Send(updateProduct, ct);
                    }
                }
                
                if (!isOnCreate)
                {
                    var productIdsToRemove = customerProductIds.Except(productIds);

                    foreach (var productId in productIdsToRemove)
                    {
                        var updateProduct = new UpdateProductCommand(
                            productId, null, null, 
                            customerRole == CustomerRole.Owner ? new UpsertCustomerCommand(new CustomerId(string.Empty)) : null, 
                            customerRole == CustomerRole.Dealer ? new UpsertCustomerCommand(new CustomerId(string.Empty)) : null,
                            null, null, null, null, null, 
                            null, null, null, null, 
                            null, null, updateBy, updateAt, false);

                        await _mediator.Send(updateProduct, ct);

                        customerProductIds.Remove(productId);
                    }
                }
            }

            if (!isOnCreate || roleUpdated)
            {
                var productsToUpdate = products?.Where(p => customerProductIds.Contains(p.ProductId)).ToList();
            
                var updatedProductOwnerId = productOwnerId;
                var updatedProductDealerId = productDealerId;

                if (roleUpdated)
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
                        p.DateOfDemandForCompensation, p.DemanderFullName, updateBy, updateAt, false, false, roleUpdated);

                    await _mediator.Send(updateProduct, ct);
                }
            }
        }

        if (customer.CustomerOrders.Any() && updateMessage is not null)
        {
            _unitOfWork.CustomerUpdateMessages.Add(updateMessage);
        }
        
        if (!saveChanges)
        {
            return customer;
        }

        await _unitOfWork.SaveChangesAsync(ct);
        _subscriptionManager.UnsubscribeFromCustomerEvents(customer);

        var result = await _customerRepository.ByStreamIdAsync(customerId, ct);
        if (result is null) return Error.Unexpected(
            nameof(CustomerId), $"{nameof(Customer)} stream with id {customerId} is not found.");

        return result;
    }
}