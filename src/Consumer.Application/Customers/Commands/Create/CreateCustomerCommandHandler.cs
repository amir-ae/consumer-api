using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Customers.Commands.Update;
using Consumer.Application.Products.Commands.Create;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Create;

public sealed class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ISender _mediator;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, ISender mediator)
    {
        _customerRepository = customerRepository;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Customer>> Handle(CreateCustomerCommand command, CancellationToken ct = default)
    {
        var (firstName, middleName, lastName, phoneNumber, cityId, address, role, 
            products, orders, createBy, createAt) = command;

        if (firstName.Length == 1) firstName = firstName.ToUpper() + '.';
        if (middleName?.Length == 1) middleName = middleName.ToUpper() + '.';
        var initials = firstName.Length == 2 && firstName[1] == '.' && middleName?.Length == 2 && middleName[1] == '.';
        
        var fullName = role == CustomerRole.Dealer ? 
            firstName : string.Concat(lastName, " ", firstName, initials ? "" : " ", middleName).Trim();

        var customer = await _customerRepository.ByDataAsync(fullName, phoneNumber, ct);
        
        var isNewCustomer = customer is null;
        var customerId = customer is null ? new CustomerId(Guid.NewGuid().ToString()) : customer.Id;
        var productIds = products?.Select(p => p.ProductId).ToHashSet();
        Func<CustomerCreatedEvent, CancellationToken, Task<Customer>> create = _customerRepository.CreateAsync;
        Action<CustomerEvent, int?> append = _customerRepository.Append;
        
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
                create,
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
                role is CustomerRole.Owner ? new UpsertCustomerCommand(customerId) : null,
                role is CustomerRole.Dealer ? new UpsertCustomerCommand(customerId) : null,
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
                createAt);

            await _mediator.Send(createProduct, ct);
        }

        if (isNewCustomer)
        {
            return customer;
        }
        
        customer.Undelete(createBy, append);

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
            true);
                
        return await _mediator.Send(updateCustomer, ct);
    }
}