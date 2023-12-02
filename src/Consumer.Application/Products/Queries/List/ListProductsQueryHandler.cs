using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Mapster;

namespace Consumer.Application.Products.Queries.List;

public sealed class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, ErrorOr<ProductForListingResponse[]>>
{
    private readonly IProductRepository _productRepository;

    public ListProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<ProductForListingResponse[]>> Handle(ListProductsQuery query, CancellationToken ct = default)
    {
        var centreId = query.CentreId;
        
        var products = await _productRepository.ListAsync(centreId, ct);

        return products.Adapt<ProductForListingResponse[]>();
    }
}