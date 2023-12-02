using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Products;
using Mapster;

namespace Consumer.Application.Products.Queries.ById;

public sealed class ProductByIdQueryHandler : IRequestHandler<ProductByIdQuery, ErrorOr<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public ProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(ProductByIdQuery query, CancellationToken ct = default)
    {
        var product = await _productRepository.ByIdAsync(query.ProductId, ct);
        
        if (product is null) return Error.NotFound(
            nameof(query.ProductId), $"{nameof(Product)} with id {query.ProductId} is not found");
        
        return product.Adapt<ProductResponse>();
    }
}