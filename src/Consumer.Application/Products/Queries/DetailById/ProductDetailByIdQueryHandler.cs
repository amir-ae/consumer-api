using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Products;
using Mapster;

namespace Consumer.Application.Products.Queries.DetailById;

public sealed class ProductDetailByIdQueryHandler : IRequestHandler<ProductDetailByIdQuery, ErrorOr<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEnrichmentService _enrichmentService;

    public ProductDetailByIdQueryHandler(IProductRepository productRepository, IEnrichmentService enrichmentService)
    {
        _productRepository = productRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(ProductDetailByIdQuery query, CancellationToken ct = default)
    {
        var product = await _productRepository.DetailByIdAsync(query.ProductId, ct);
        
        if (product is null) return Error.NotFound(
            nameof(query.ProductId), $"{nameof(Product)} with id {query.ProductId} is not found");

        var productResult = product.Adapt<ProductResponse>();
        
        return await _enrichmentService.EnrichProductResponse(productResult, ct);
    }
}