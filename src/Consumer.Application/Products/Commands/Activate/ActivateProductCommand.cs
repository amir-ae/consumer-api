using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Activate;

public record ActivateProductCommand(
    AppUserId AppUserId,
    ProductId ProductId) : IRequest<ErrorOr<ProductResponse>>;