using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Domain.Products.ValueObjects;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Products.Queries.List;

public record ListProductsQuery(
    CentreId? CentreId) : IRequest<ErrorOr<ProductForListingResponse[]>>;