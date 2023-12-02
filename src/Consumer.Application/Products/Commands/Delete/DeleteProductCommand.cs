using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Delete;

public record DeleteProductCommand(
    ProductId ProductId,
    AppUserId DeleteBy) : IRequest<ErrorOr<Product>>;