using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Customers.Commands.Update;
using Consumer.Application.Products.Commands.Create;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Mapster;

namespace Consumer.Application.Customers.Commands.Create;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ErrorOr<CustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ISender _mediator;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, ISender mediator)
    {
        _customerRepository = customerRepository;
        _mediator = mediator;
    }

    public async Task<ErrorOr<CustomerResponse>> Handle(CreateCustomerCommand command, CancellationToken ct = default)
    {
        var (appUserId, firstName, middleName, lastName, phoneNumber, cityId, address, role, 
            products, createdAt) = command;

        if (firstName.Length == 1) firstName = firstName.ToUpper() + '.';
        if (middleName?.Length == 1) middleName = middleName.ToUpper() + '.';
        var initials = firstName.Length == 2 && firstName[1] == '.' && middleName?.Length == 2 && middleName[1] == '.';
        
        var fullName = role == CustomerRole.Dealer ? 
            firstName : string.Concat(lastName, " ", firstName, initials ? "" : " ", middleName).Trim();

        var customer = await _customerRepository.ByDataAsync(fullName, phoneNumber, ct);
        
        var customerId = customer?.Id ?? new CustomerId(Guid.NewGuid().ToString());

        foreach (var p in products ?? new())
        {
            if (p.IsId) continue;
            
            var createProductCommand = new CreateProductCommand(
                appUserId,
                p.ProductId,
                p.Brand ?? string.Empty,
                p.Model ?? string.Empty,
                p.SerialId,
                role is CustomerRole.Owner ? new Customer(customerId) : null,
                role is CustomerRole.Dealer ? new Customer(customerId) : null,
                p.DeviceType,
                p.PanelModel,
                p.PanelSerialNumber,
                p.WarrantyCardNumber,
                p.DateOfPurchase,
                p.InvoiceNumber,
                p.PurchasePrice,
                p.Orders,
                p.IsUnrepairable,
                p.DateOfDemandForCompensation,
                p.DemanderFullName,
                createdAt);

            await _mediator.Send(createProductCommand, ct);
        }

        if (customer is not null)
        {
            if (customer.IsDeleted)
            {
                var customerUndeletedEvent = new CustomerUndeletedEvent(
                    customerId,
                    appUserId,
                    createdAt);

                await _customerRepository.UndeleteAsync(customerUndeletedEvent, ct);
            }
            
            var updateCommand = new UpdateCustomerCommand(
                appUserId,
                customerId,
                firstName,
                middleName,
                lastName,
                phoneNumber,
                cityId,
                address,
                role,
                products,
                createdAt,
                true);
                
            return await _mediator.Send(updateCommand, ct);
        }
        
        var productIds = products?.Select(p => p.ProductId).ToHashSet();
        
        var customerCreatedEvent = new CustomerCreatedEvent(
            customerId,
            firstName,
            middleName,
            lastName,
            fullName,
            phoneNumber,
            cityId,
            address,
            role,
            productIds,
            appUserId,
            createdAt);

        var customerResult = await _customerRepository.CreateAsync(customerCreatedEvent, ct);

        return customerResult.Adapt<CustomerResponse>();
    }
}