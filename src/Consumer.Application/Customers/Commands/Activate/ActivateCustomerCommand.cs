using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Activate;

public record ActivateCustomerCommand(
    CustomerId CustomerId,
    AppUserId ActivateBy) : IRequest<ErrorOr<Customer>>;