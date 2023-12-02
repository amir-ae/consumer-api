using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Deactivate;

public record DeactivateProductCommand(
    ProductId ProductId,
    AppUserId DeactivateBy) : IRequest<ErrorOr<Product>>;