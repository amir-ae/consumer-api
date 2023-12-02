using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Queries.DetailById;

public record CustomerDetailByIdQuery(
    CustomerId CustomerId) : IRequest<ErrorOr<CustomerResponse>>;