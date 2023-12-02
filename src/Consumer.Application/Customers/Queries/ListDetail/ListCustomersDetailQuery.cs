using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Domain.Products.ValueObjects;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Customers.Queries.ListDetail;

public record ListCustomersDetailQuery(
    CentreId? CentreId) : IRequest<ErrorOr<CustomerResponse[]>>;