using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Queries.ById;

public record CustomerByIdQuery(
    CustomerId CustomerId) : IRequest<ErrorOr<CustomerResponse>>;