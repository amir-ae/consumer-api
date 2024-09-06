using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Customers.Commands.Create;
using Commerce.Application.Customers.Commands.Update;
using MediatR;
using ErrorOr;
using Commerce.Domain.Customers;
using Mapster;

namespace Commerce.Application.Customers.Commands.Upsert;

public sealed class UpsertCustomerCommandHandler : IRequestHandler<UpsertCustomerCommand, ErrorOr<Customer>>
{
    private readonly ISender _mediator;
    private readonly ICustomerRepository _customerRepository;

    public UpsertCustomerCommandHandler(ISender mediator, ICustomerRepository customerRepository)
    {
        _mediator = mediator;
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<Customer>> Handle(UpsertCustomerCommand command, CancellationToken ct = default)
    {
        var customerId = command.CustomerId;

        if (customerId is not null && await _customerRepository.CheckByIdAsync(customerId, ct))
        {
            var updateCustomerCommand = command.Adapt<UpdateCustomerCommand>();
            return await _mediator.Send(updateCustomerCommand, ct);
        }

        var createCustomerCommand = command.Adapt<CreateCustomerCommand>();
        return await _mediator.Send(createCustomerCommand, ct);
    }
}