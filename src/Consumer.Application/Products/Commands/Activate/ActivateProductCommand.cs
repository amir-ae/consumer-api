using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Activate;

public record ActivateProductCommand(
    ProductId ProductId,
    AppUserId ActivateBy) : IRequest<ErrorOr<Product>>;