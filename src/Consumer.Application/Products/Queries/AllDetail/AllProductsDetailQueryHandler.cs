using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;
using ValueTaskSupplement;

namespace Consumer.Application.Products.Queries.AllDetail;

public sealed class AllProductsDetailQueryHandler : IRequestHandler<AllProductsDetailQuery, ErrorOr<ProductResponse[]>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEnrichmentService _enrichmentService;

    public AllProductsDetailQueryHandler(IProductRepository productRepository, IEnrichmentService enrichmentService)
    {
        _productRepository = productRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<ProductResponse[]>> Handle(AllProductsDetailQuery query, CancellationToken ct = default)
    {
        var products = await _productRepository.AllDetailAsync(ct);

        var tasks = products.Adapt<List<ProductResponse>>()
            .Select(product => _enrichmentService.EnrichProductResponse(product, ct));
        
        return await ValueTaskEx.WhenAll(tasks);
    }
}