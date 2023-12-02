using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers.Events;
using Mapster;

namespace Consumer.Application.Customers.Commands.Deactivate;

public sealed class DeactivateCustomerCommandHandler : IRequestHandler<DeactivateCustomerCommand, ErrorOr<CustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public DeactivateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<CustomerResponse>> Handle(DeactivateCustomerCommand command, CancellationToken ct = default)
    {
        var (appUserId, customerId) = command;

        var customerDeactivatedEvent = new CustomerDeactivatedEvent(
            customerId,
            appUserId);

        var customer = await _customerRepository.DeactivateAsync(customerDeactivatedEvent, ct);

        return customer.Adapt<CustomerResponse>();
    }
}