using FluentValidation;

namespace Consumer.Application.Products.Queries.DetailById;

public sealed class ProductDetailByIdQueryValidator : AbstractValidator<ProductDetailByIdQuery>
{
    public ProductDetailByIdQueryValidator()
    {
        RuleFor(x => x.ProductId.Value).NotEmpty().MaximumLength(100);
    }
}