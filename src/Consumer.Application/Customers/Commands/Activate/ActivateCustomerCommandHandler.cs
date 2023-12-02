using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Activate;

public sealed class ActivateCustomerCommandHandler : IRequestHandler<ActivateCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;

    public ActivateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<Customer>> Handle(ActivateCustomerCommand command, CancellationToken ct = default)
    {
        var (customerId, activateBy) = command;

        var customer = await _customerRepository.ByIdAsync(customerId, ct);
        if (customer is null) return Error.NotFound(
            nameof(CustomerId), $"{nameof(Customer)} with id {customerId} is not found.");

        customer = customer.Activate(activateBy, _customerRepository.Append);

        await _customerRepository.SaveChangesAsync(ct);

        return customer;
    }
}