using FluentValidation;

namespace Consumer.Application.Products.Queries.ByPage;


public sealed class ProductsByPageQueryValidator : AbstractValidator<ProductsByPageQuery>
{
    public ProductsByPageQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .NotEmpty();
        RuleFor(x => x.PageSize)
            .NotEmpty();
    }
}