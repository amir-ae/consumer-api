using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Queries.DetailById;

public record ProductDetailByIdQuery(
    ProductId ProductId) : IRequest<ErrorOr<ProductResponse>>;