using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Application.Common.Interfaces.Persistence;
using MediatR;
using ErrorOr;
using Mapster;

namespace Consumer.Application.Products.Queries.EventsById;

public sealed class ProductEventsByIdQueryHandler : IRequestHandler<ProductEventsByIdQuery, ErrorOr<ProductEventsResponse>>
{
    private readonly IProductRepository _productRepository;

    public ProductEventsByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<ProductEventsResponse>> Handle(ProductEventsByIdQuery query, CancellationToken ct = default)
    {
        var productId = query.ProductId;
        
        var productEvents = await _productRepository.EventsByIdAsync(productId, ct);

        return productEvents.Adapt<ProductEventsResponse>();
    }
}