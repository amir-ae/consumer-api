using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Products.Events;
using Mapster;

namespace Consumer.Application.Products.Commands.Delete;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ErrorOr<ProductResponse>>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(DeleteProductCommand command, CancellationToken ct = default)
    {
        var (appUserId, productId) = command;

        var productDeletedEvent = new ProductDeletedEvent(
            productId,
            appUserId);

        var product = await _productRepository.DeleteAsync(productDeletedEvent, ct);
        
        return product.Adapt<ProductResponse>();
    }
}