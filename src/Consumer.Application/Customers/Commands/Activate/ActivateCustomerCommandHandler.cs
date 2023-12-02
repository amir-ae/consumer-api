using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Activate;

public sealed class ActivateCustomerCommandHandler : IRequestHandler<ActivateCustomerCommand, ErrorOr<Customer>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;

    public ActivateCustomerCommandHandler(ICustomerRepository customerRepository, IEventSubscriptionManager subscriptionManager)
    {
        _customerRepository = customerRepository;
        _subscriptionManager = subscriptionManager;
    }

    public async Task<ErrorOr<Customer>> Handle(ActivateCustomerCommand command, CancellationToken ct = default)
    {
        var (customerId, activateBy) = command;

        var customer = await _customerRepository.ByIdAsync(customerId, ct);
        if (customer is null) return Error.NotFound(
            nameof(CustomerId), $"{nameof(Customer)} with id {customerId} is not found.");

        _subscriptionManager.SubscribeToCustomerEvents(customer);
        
        customer.Activate(activateBy);

        await _customerRepository.SaveChangesAsync(ct);
        _subscriptionManager.UnsubscribeFromCustomerEvents(customer);

        var result = await _customerRepository.ByStreamIdAsync(customerId, ct);
        if (result is null) return Error.Unexpected(
            nameof(CustomerId), $"{nameof(Customer)} stream with id {customerId} is not found.");

        return result;
    }
}