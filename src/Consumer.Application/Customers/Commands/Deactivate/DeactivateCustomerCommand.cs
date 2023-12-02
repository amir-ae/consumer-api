using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Deactivate;

public record DeactivateCustomerCommand(
    CustomerId CustomerId,
    AppUserId DeactivateBy) : IRequest<ErrorOr<Customer>>;