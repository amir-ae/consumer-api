using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Products.Queries.AllDetail;

public record AllProductsDetailQuery() : IRequest<ErrorOr<ProductResponse[]>>;