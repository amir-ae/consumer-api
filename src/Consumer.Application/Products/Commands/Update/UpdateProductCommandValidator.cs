using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;
using FluentValidation;

namespace Consumer.Application.Products.Commands.Update;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    
    public UpdateProductCommandValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;
        RuleFor(x => x.AppUserId.Value).NotEmpty();
        RuleFor(x => x.ProductId.Value).NotEmpty();
    }
}