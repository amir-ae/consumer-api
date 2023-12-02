using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;

namespace Consumer.Application.Products.Queries.AllDetail;

public sealed class AllProductsDetailQueryHandler : IRequestHandler<AllProductsDetailQuery, ErrorOr<List<ProductResponse>>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEnrichmentService _enrichmentService;

    public AllProductsDetailQueryHandler(IProductRepository productRepository, IEnrichmentService enrichmentService)
    {
        _productRepository = productRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<List<ProductResponse>>> Handle(AllProductsDetailQuery query, CancellationToken ct = default)
    {
        var products = await _productRepository.AllDetailAsync(ct);

        var tasks = products.Adapt<List<ProductResponse>>()
            .Select(async product => await _enrichmentService.EnrichProductResponse(product, ct));
        
        var result = await Task.WhenAll(tasks);
        return result.ToList();
    }
}