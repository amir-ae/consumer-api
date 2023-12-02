using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Delete;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IEventSubscriptionManager subscriptionManager)
    {
        _customerRepository = customerRepository;
        _subscriptionManager = subscriptionManager;
    }

    public async Task<ErrorOr<Customer>> Handle(DeleteCustomerCommand command, CancellationToken ct = default)
    {
        var (customerId, deleteBy) = command;

        var customer = await _customerRepository.ByIdAsync(customerId, ct);
        if (customer is null) return Error.NotFound(
            nameof(CustomerId), $"{nameof(Customer)} with id {customerId} is not found.");

        _subscriptionManager.SubscribeToCustomerEvents(customer);
        
        customer.Delete(deleteBy);
        
        await _customerRepository.SaveChangesAsync(ct);
        _subscriptionManager.UnsubscribeFromCustomerEvents(customer);

        var result = await _customerRepository.ByStreamIdAsync(customerId, ct);
        if (result is null) return Error.Unexpected(
            nameof(CustomerId), $"{nameof(Customer)} stream with id {customerId} is not found.");

        return result;
    }
}