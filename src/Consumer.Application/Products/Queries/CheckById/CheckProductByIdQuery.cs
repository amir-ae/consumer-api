using Consumer.Domain.Products.ValueObjects;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Products.Queries.CheckById;

public record CheckProductByIdQuery(
    ProductId ProductId) : IRequest<ErrorOr<bool>>;