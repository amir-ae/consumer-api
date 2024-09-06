using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Products.Commands.Create;
using Commerce.Application.Products.Commands.Update;
using MediatR;
using ErrorOr;
using Commerce.Domain.Products;
using Mapster;

namespace Commerce.Application.Products.Commands.Upsert;

public sealed class UpsertProductCommandHandler : IRequestHandler<UpsertProductCommand, ErrorOr<Product>>
{
    private readonly ISender _mediator;
    private readonly IProductRepository _productRepository;

    public UpsertProductCommandHandler(ISender mediator, IProductRepository productRepository)
    {
        _mediator = mediator;
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<Product>> Handle(UpsertProductCommand command, CancellationToken ct = default)
    {
        var productId = command.ProductId;
        var brand = command.Brand;
        var model = command.Model;

        if ((string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(model))
            && await _productRepository.CheckByIdAsync(productId, ct))
        {
            var updateProductCommand = command.Adapt<UpdateProductCommand>();
            return await _mediator.Send(updateProductCommand, ct);
        }

        var createProductCommand = command.Adapt<CreateProductCommand>();
        return await _mediator.Send(createProductCommand, ct);
    }
}