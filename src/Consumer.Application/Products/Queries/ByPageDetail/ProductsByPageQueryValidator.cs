using FluentValidation;

namespace Consumer.Application.Products.Queries.ByPageDetail;

public sealed class ProductsByPageDetailQueryValidator : AbstractValidator<ProductsByPageDetailQuery>
{
    public ProductsByPageDetailQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}