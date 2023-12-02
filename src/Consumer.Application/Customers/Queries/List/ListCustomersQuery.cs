using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Domain.Products.ValueObjects;
using ErrorOr;
using MediatR;

namespace Consumer.Application.Customers.Queries.List;

public record ListCustomersQuery(
    CentreId? CentreId) : IRequest<ErrorOr<CustomerForListingResponse[]>>;