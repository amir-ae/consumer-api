using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Queries.ById;

public record ProductByIdQuery(
    ProductId ProductId) : IRequest<ErrorOr<ProductResponse>>;