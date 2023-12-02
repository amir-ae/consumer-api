using FluentValidation;

namespace Consumer.Application.Products.Queries.ByPage;

public sealed class ProductsByPageQueryValidator : AbstractValidator<ProductsByPageQuery>
{
    public ProductsByPageQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}