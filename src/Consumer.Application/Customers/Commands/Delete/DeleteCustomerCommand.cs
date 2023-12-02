using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Delete;

public record DeleteCustomerCommand(
    CustomerId CustomerId,
    AppUserId DeleteBy) : IRequest<ErrorOr<Customer>>;