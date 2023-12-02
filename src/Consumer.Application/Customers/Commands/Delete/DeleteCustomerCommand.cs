using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Delete;

public record DeleteCustomerCommand(
    AppUserId AppUserId,
    CustomerId CustomerId) : IRequest<ErrorOr<CustomerResponse>>;