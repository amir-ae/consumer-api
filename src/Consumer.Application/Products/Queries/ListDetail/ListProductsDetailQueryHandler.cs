using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;
using ValueTaskSupplement;

namespace Consumer.Application.Products.Queries.ListDetail;

public sealed class ListProductsDetailQueryHandler : IRequestHandler<ListProductsDetailQuery, ErrorOr<ProductResponse[]>>
{
    private readonly IProductRepository _productRepository;
    private readonly IEnrichmentService _enrichmentService;

    public ListProductsDetailQueryHandler(IProductRepository productRepository, IEnrichmentService enrichmentService)
    {
        _productRepository = productRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<ProductResponse[]>> Handle(ListProductsDetailQuery query, CancellationToken ct = default)
    {
        var centreId = query.CentreId;
        
        var products = await _productRepository.ListDetailAsync(centreId, ct);

        var tasks = products.Adapt<List<ProductResponse>>()
            .Select(product => _enrichmentService.EnrichProductResponse(product, ct));
        
        return await ValueTaskEx.WhenAll(tasks);
    }
}