using System.Collections.Concurrent;
using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;

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

        var productResponses = new ConcurrentBag<ProductResponse>();

        await Parallel.ForEachAsync(products, ct, async (product, token) =>
        {
            var productResponse = product.Adapt<ProductResponse>();
            productResponses.Add(await _enrichmentService.EnrichProductResponse(productResponse, token));
        });

        return productResponses
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .ToArray();
    }
}