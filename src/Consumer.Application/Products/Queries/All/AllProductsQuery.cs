using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Products.Queries.All;

public record AllProductsQuery() : IRequest<ErrorOr<ProductForListingResponse[]>>;