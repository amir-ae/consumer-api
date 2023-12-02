using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Deactivate;

public sealed class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;

    public DeactivateProductCommandHandler(IProductRepository productRepository, IEventSubscriptionManager subscriptionManager)
    {
        _productRepository = productRepository;
        _subscriptionManager = subscriptionManager;
    }

    public async Task<ErrorOr<Product>> Handle(DeactivateProductCommand command, CancellationToken ct = default)
    {
        var (productId, deactivateBy) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        if (product is null) return Error.NotFound(
            nameof(ProductId), $"{nameof(Product)} with id {productId} is not found.");

        _subscriptionManager.SubscribeToProductEvents(product);
        
        product.Deactivate(deactivateBy, _productRepository.Append);
        
        await _productRepository.SaveChangesAsync(ct);
        _subscriptionManager.UnsubscribeFromProductEvents(product);
        
        var result = await _productRepository.ByStreamIdAsync(productId, ct);
        if (result is null) return Error.Unexpected(
            nameof(ProductId), $"{nameof(Product)} stream with id {productId} is not found.");

        return result;
    }
}