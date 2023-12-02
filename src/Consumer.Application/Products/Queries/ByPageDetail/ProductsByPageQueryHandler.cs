using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;
using ValueTaskSupplement;

namespace Consumer.Application.Products.Queries.ByPageDetail;

public sealed class ProductsByPageDetailQueryHandler : IRequestHandler<ProductsByPageDetailQuery, ErrorOr<PaginatedList<ProductResponse>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEnrichmentService _enrichmentService;

    public ProductsByPageDetailQueryHandler(IProductRepository productRepository, IEnrichmentService enrichmentService)
    {
        _productRepository = productRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<PaginatedList<ProductResponse>>> Handle(ProductsByPageDetailQuery query, CancellationToken ct = default)
    {
        var (pageSize, pageIndex, nextPage, keyId, centreId) = query;

        var (products, totalCount) = await _productRepository.ByPageDetailAsync(pageSize, pageIndex, nextPage, keyId, centreId, ct);

        var tasks = products.Adapt<List<ProductResponse>>()
            .Select(product => _enrichmentService.EnrichProductResponse(product, ct));
        
        var result = await ValueTaskEx.WhenAll(tasks);

        return new PaginatedList<ProductResponse>(
            pageIndex, pageSize, totalCount, result);
    }
}