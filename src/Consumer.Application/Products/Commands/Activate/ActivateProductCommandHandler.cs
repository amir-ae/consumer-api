using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Products.Events;
using Mapster;

namespace Consumer.Application.Products.Commands.Activate;

public sealed class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand, ErrorOr<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public ActivateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(ActivateProductCommand command, CancellationToken ct = default)
    {
        var (appUserId, productId) = command;

        var productActivatedEvent = new ProductActivatedEvent(
            productId,
            appUserId);

        var product = await _productRepository.ActivateAsync(productActivatedEvent, ct);
        
        return product.Adapt<ProductResponse>();
    }
}