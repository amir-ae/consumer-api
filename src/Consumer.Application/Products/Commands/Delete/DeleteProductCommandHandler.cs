using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Delete;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;

    public DeleteProductCommandHandler(IProductRepository productRepository, IEventSubscriptionManager subscriptionManager)
    {
        _productRepository = productRepository;
        _subscriptionManager = subscriptionManager;
    }

    public async Task<ErrorOr<Product>> Handle(DeleteProductCommand command, CancellationToken ct = default)
    {
        var (productId, deleteBy) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        if (product is null) return Error.NotFound(
            nameof(ProductId), $"{nameof(Product)} with id {productId} is not found.");

        _subscriptionManager.SubscribeToProductEvents(product);
        
        product.Delete(deleteBy);

        await _productRepository.SaveChangesAsync(ct);
        _subscriptionManager.UnsubscribeFromProductEvents(product);
        
        var result = await _productRepository.ByStreamIdAsync(productId, ct);
        if (result is null) return Error.Unexpected(
            nameof(ProductId), $"{nameof(Product)} stream with id {productId} is not found.");

        return result;
    }
}