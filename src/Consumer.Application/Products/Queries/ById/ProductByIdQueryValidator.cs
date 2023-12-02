using FluentValidation;

namespace Consumer.Application.Products.Queries.ById;

public sealed class ProductByIdQueryValidator : AbstractValidator<ProductByIdQuery>
{
    public ProductByIdQueryValidator()
    {
        RuleFor(x => x.ProductId.Value).NotEmpty().MaximumLength(100);
    }
}