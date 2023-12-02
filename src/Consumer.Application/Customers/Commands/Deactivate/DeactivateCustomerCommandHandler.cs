using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Deactivate;

public sealed class DeactivateCustomerCommandHandler : IRequestHandler<DeactivateCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;

    public DeactivateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<Customer>> Handle(DeactivateCustomerCommand command, CancellationToken ct = default)
    {
        var (customerId, deactivateBy) = command;

        var customer = await _customerRepository.ByIdAsync(customerId, ct);
        if (customer is null) return Error.NotFound(
            nameof(CustomerId), $"{nameof(Customer)} with id {customerId} is not found.");
        
        customer.Deactivate(deactivateBy, _customerRepository.Append);
        
        await _customerRepository.SaveChangesAsync(ct);

        var result = await _customerRepository.ByStreamIdAsync(customerId, ct);
        if (result is null) return Error.Unexpected(
            nameof(CustomerId), $"{nameof(Customer)} stream with id {customerId} is not found.");

        return result;
    }
}