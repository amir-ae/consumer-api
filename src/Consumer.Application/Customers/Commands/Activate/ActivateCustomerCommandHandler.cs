using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers.Events;
using Mapster;

namespace Consumer.Application.Customers.Commands.Activate;

public sealed class ActivateCustomerCommandHandler : IRequestHandler<ActivateCustomerCommand, ErrorOr<CustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public ActivateCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<CustomerResponse>> Handle(ActivateCustomerCommand command, CancellationToken ct = default)
    {
        var (appUserId, customerId) = command;

        var customerActivatedEvent = new CustomerActivatedEvent(
            customerId,
            appUserId);

        var customer = await _customerRepository.ActivateAsync(customerActivatedEvent, ct);

        return customer.Adapt<CustomerResponse>();
    }
}