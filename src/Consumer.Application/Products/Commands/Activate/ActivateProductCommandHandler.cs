using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Activate;

public sealed class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository;

    public ActivateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<Product>> Handle(ActivateProductCommand command, CancellationToken ct = default)
    {
        var (productId, activateBy) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        if (product is null) return Error.NotFound(
            nameof(ProductId), $"{nameof(Product)} with id {productId} is not found.");
        
        product = product.Activate(activateBy, _productRepository.Append);
        
        await _productRepository.SaveChangesAsync(ct);
        
        return product;
    }
}