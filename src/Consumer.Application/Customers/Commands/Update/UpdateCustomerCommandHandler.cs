using Consumer.API.Contract.V1.Customers.Messages;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Application.Products.Commands.Create;
using Consumer.Application.Products.Commands.Update;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Mapster;

namespace Consumer.Application.Customers.Commands.Update;

public sealed class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ErrorOr<CustomerResponse>>
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

    public async Task<ErrorOr<CustomerResponse>> Handle(UpdateCustomerCommand command, CancellationToken ct = default)
    {
        var (appUserId, customerId, firstName, middleName, lastName, phoneNumber, 
            cityId, address, role, products, updatedAt, onCreate) = command;

        var customer = await _customerRepository.ByIdAsync(customerId, ct);
        
        if (customer is null) return Error.NotFound(
            nameof(CustomerId), $"{nameof(Domain.Customers.Customer)} with id {customerId} is not found.");

        CustomerUpdateMessage? customerUpdateMessage = null;
        
        if (!string.IsNullOrEmpty(firstName) && firstName != customer.FirstName
            || !string.IsNullOrEmpty(middleName) && middleName != customer.MiddleName
            || !string.IsNullOrEmpty(lastName) && lastName != customer.LastName)
        {
            var customerNameChangedEvent = new CustomerNameChangedEvent(
                customerId,
                firstName ?? customer.FirstName,
                middleName,
                lastName ?? customer.LastName,
                null,
                appUserId,
                updatedAt);
            
            customer = await _customerRepository.UpdateAsync(customerNameChangedEvent, customer, ct);

            customerUpdateMessage = new CustomerUpdateMessage(appUserId.Value, customerId.Value,
                customerNameChangedEvent.FullName, updatedAt);
        }
        
        if (role is not null && role != customer.Role)
        {
            var customerRoleChangedEvent = new CustomerRoleChangedEvent(
                customerId,
                role.Value,
                appUserId,
                updatedAt);
            
            customer = await _customerRepository.UpdateAsync(customerRoleChangedEvent, customer, ct);

            foreach (var productId in customer.ProductIds.ToList())
            {
                var updateProductCommand = new UpdateProductCommand(
                    appUserId, productId, null, null, 
                    customer.Role is CustomerRole.Owner ? new Customer(customerId) : new Customer(new CustomerId(string.Empty)), 
                    customer.Role is CustomerRole.Dealer ? new Customer(customerId) : new Customer(new CustomerId(string.Empty)),
                    null, null, null, null, null, 
                    null, null, null, null, 
                    null, null, null, false, true);

                await _mediator.Send(updateProductCommand, ct);
            }
        }
        
        if (!string.IsNullOrEmpty(phoneNumber) && phoneNumber != customer.PhoneNumber)
        {
            var customerUpdatedEvent = new CustomerPhoneNumberChangedEvent(
                customerId,
                phoneNumber,
                appUserId,
                updatedAt);
            
            customer = await _customerRepository.UpdateAsync(customerUpdatedEvent, customer, ct);
        }
        
        if (cityId is not null && cityId != customer.CityId
            || !string.IsNullOrEmpty(address) && address != customer.Address)
        {
            var customerUpdatedEvent = new CustomerAddressChangedEvent(
                customerId,
                cityId ?? customer.CityId,
                address ?? customer.Address,
                appUserId,
                updatedAt);
            
            customer = await _customerRepository.UpdateAsync(customerUpdatedEvent, customer, ct);
        }
        
        var productIds = products?.Select(p => p.ProductId).ToHashSet();
        
        if (productIds is not null)
        {
            var productsToUpdate = products?.Where(p => customer.ProductIds.Contains(p.ProductId)).ToList();
            
            foreach (var p in productsToUpdate ?? new())
            {
                var updateProductCommand = new UpdateProductCommand(
                    appUserId, p.ProductId, p.Brand, p.Model,
                    customer.Role is CustomerRole.Owner ? new Customer(customerId) : null, 
                    customer.Role is CustomerRole.Dealer ? new Customer(customerId) : null,
                    p.DeviceType, p.PanelModel, p.PanelSerialNumber, p.WarrantyCardNumber, p.DateOfPurchase, 
                    p.InvoiceNumber, p.PurchasePrice, p.Orders, p.IsUnrepairable, 
                    p.DateOfDemandForCompensation, p.DemanderFullName, updatedAt);

                await _mediator.Send(updateProductCommand, ct);
            }
            
            if (!productIds.SetEquals(customer.ProductIds))
            {
                var productIdsToAdd = productIds.Except(customer.ProductIds).ToList();
                var productsToAdd = products?.Where(p => productIdsToAdd.Contains(p.ProductId)).ToList();
            
                foreach (var p in productsToAdd ?? new())
                {
                    if (!p.IsId)
                    {
                        var createProductCommand = new CreateProductCommand(
                            appUserId, p.ProductId, p.Brand!, p.Model!, p.SerialId,
                            customer.Role is CustomerRole.Owner ? new Customer(customerId) : null, 
                            customer.Role is CustomerRole.Dealer ? new Customer(customerId) : null,
                            p.DeviceType, p.PanelModel, p.PanelSerialNumber, p.WarrantyCardNumber, p.DateOfPurchase, 
                            p.InvoiceNumber, p.PurchasePrice, p.Orders, p.IsUnrepairable, 
                            p.DateOfDemandForCompensation, p.DemanderFullName, updatedAt);

                        await _mediator.Send(createProductCommand, ct);
                    }
                    else
                    {
                        var updateProductCommand = new UpdateProductCommand(
                            appUserId, p.ProductId, null, null, 
                            customer.Role is CustomerRole.Owner ? new Customer(customerId) : null, 
                            customer.Role is CustomerRole.Dealer ? new Customer(customerId) : null,
                            null, null, null, null, null, 
                            null, null, null, null, 
                            null, null, updatedAt);

                        await _mediator.Send(updateProductCommand, ct);
                    }
                }
            }

            if (!onCreate)
            {
                var productIdsToRemove = customer.ProductIds.Except(productIds);
                foreach (var productId in productIdsToRemove)
                {
                    var updateProductCommand = new UpdateProductCommand(
                        appUserId, productId, null, null, 
                        customer.Role is CustomerRole.Owner ? new Customer(new CustomerId(string.Empty)) : null, 
                        customer.Role is CustomerRole.Dealer ? new Customer(new CustomerId(string.Empty)) : null,
                        null, null, null, null, null, 
                        null, null, null, null, 
                        null, null, updatedAt);

                    await _mediator.Send(updateProductCommand, ct);
                }
            }
        }

        if (customerUpdateMessage is not null)
        {
            await _orderingService.PublishCustomerUpdateAsync(customerUpdateMessage, ct);
        }
        
        await _customerRepository.SaveChangesAsync(ct);

        return customer.Adapt<CustomerResponse>();
    }
}