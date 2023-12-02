using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Mapster;

namespace Consumer.Application.Products.Queries.All;

public sealed class AllProductsQueryHandler : IRequestHandler<AllProductsQuery, ErrorOr<List<ProductForListingResponse>>>
{
    private readonly IProductRepository _productRepository;

    public AllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<List<ProductForListingResponse>>> Handle(AllProductsQuery query, CancellationToken ct = default)
    {
        var products = await _productRepository.AllAsync(ct);

        return products.Adapt<List<ProductForListingResponse>>();
    }
}