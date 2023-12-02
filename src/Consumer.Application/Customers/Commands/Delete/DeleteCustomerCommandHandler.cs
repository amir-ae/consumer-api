using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers.Events;
using Mapster;

namespace Consumer.Application.Customers.Commands.Delete;

public sealed class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, ErrorOr<CustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<CustomerResponse>> Handle(DeleteCustomerCommand command, CancellationToken ct = default)
    {
        var (appUserId, customerId) = command;

        var customerDeletedEvent = new CustomerDeletedEvent(
            customerId,
            appUserId);

        var customer = await _customerRepository.DeleteAsync(customerDeletedEvent, ct);

        return customer.Adapt<CustomerResponse>();
    }
}