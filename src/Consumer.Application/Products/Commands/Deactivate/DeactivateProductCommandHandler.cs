using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Products.Events;
using Mapster;

namespace Consumer.Application.Products.Commands.Deactivate;

public sealed class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, ErrorOr<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public DeactivateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(DeactivateProductCommand command, CancellationToken ct = default)
    {
        var (appUserId, productId) = command;

        var productDeactivatedEvent = new ProductDeactivatedEvent(
            productId,
            appUserId);

        var product = await _productRepository.DeactivateAsync(productDeactivatedEvent, ct);
        
        return product.Adapt<ProductResponse>();
    }
}