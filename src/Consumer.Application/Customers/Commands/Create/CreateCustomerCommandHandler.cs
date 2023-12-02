using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Application.Customers.Commands.Update;
using Consumer.Application.Products.Commands.Create;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Create;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ILookupService _lookupService;
    private readonly ISender _mediator;

    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork, 
        IEventSubscriptionManager subscriptionManager, 
        ILookupService lookupService, 
        ISender mediator)
    {
        _customerRepository = unitOfWork.CustomerRepository;
        _subscriptionManager = subscriptionManager;
        _lookupService = lookupService;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Customer>> Handle(CreateCustomerCommand command, CancellationToken ct = default)
    {
        var (firstName, middleName, lastName, phoneNumber, cityId, address, role, 
            products, orders, createBy, createAt, saveChanges) = command;

        if (firstName.Length == 1) firstName = firstName.ToUpper() + '.';
        if (middleName?.Length == 1) middleName = middleName.ToUpper() + '.';
        var initials = firstName is [_, '.'] && middleName is [_, '.'];
        
        var fullName = role == CustomerRole.Dealer ? 
            firstName : string.Concat(lastName, " ", firstName, initials ? "" : " ", middleName).Trim();

        var customer = await _customerRepository.ByDataAsync(fullName, phoneNumber.Value, ct);
        
        var isNewCustomer = customer is null;
        var customerId = customer is null ? new CustomerId(Guid.NewGuid().ToString()) : customer.Id;
        var productIds = products?.Select(p => p.ProductId).ToHashSet();

        phoneNumber = await phoneNumber.InspectCountryCode(_lookupService.CountryCode, ct);
        
        if (customer is null)
        {
            customer = await Customer.CreateAsync(
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
                orders,
                createBy,
                createAt,
                _customerRepository.Create,
                _customerRepository.CreateAsync,
                saveChanges,
                ct);
        }
        
        foreach (var p in products ?? new())
        {
            if (p.IsId) continue;
            
            var createProduct = new CreateProductCommand(
                p.ProductId,
                p.Brand ?? string.Empty,
                p.Model ?? string.Empty,
                p.SerialId,
                role == CustomerRole.Owner ? new UpsertCustomerCommand(customerId) : null,
                role == CustomerRole.Dealer ? new UpsertCustomerCommand(customerId) : null,
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
                createBy,
                createAt,
                saveChanges);

            await _mediator.Send(createProduct, ct);
        }

        if (isNewCustomer)
        {
            return customer;
        }
        
        _subscriptionManager.SubscribeToCustomerEvents(customer);
        customer.Undelete(createBy);

        var updateCustomer = new UpdateCustomerCommand(
            customerId,
            firstName,
            middleName,
            lastName,
            phoneNumber,
            cityId,
            address,
            role,
            productIds?.Select(id => new UpsertProductCommand(id)).ToHashSet(),
            orders,
            createBy,
            createAt,
            saveChanges,
            true);
                
        return await _mediator.Send(updateCustomer, ct);
    }
}