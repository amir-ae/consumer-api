using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Products;
using Mapster;

namespace Consumer.Application.Products.Queries.DetailByOrderId;

public sealed class ProductDetailByIdQueryHandler : IRequestHandler<ProductDetailByOrderIdQuery, ErrorOr<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEnrichmentService _enrichmentService;

    public ProductDetailByIdQueryHandler(IProductRepository productRepository, IEnrichmentService enrichmentService)
    {
        _productRepository = productRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(ProductDetailByOrderIdQuery query, CancellationToken ct = default)
    {
        var product = await _productRepository.DetailByOrderIdAsync(query.OrderId, ct);

        if (product is null) return Error.NotFound(
            nameof(query.OrderId), $"{nameof(Product)} with order id {query.OrderId} is not found");

        var productResult = product.Adapt<ProductResponse>();
        
        return await _enrichmentService.EnrichProductResponse(productResult, ct);
    }
}