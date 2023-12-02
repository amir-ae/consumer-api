using FluentValidation;

namespace Consumer.Application.Products.Queries.ByPageDetail;


public sealed class ProductsByPageDetailQueryValidator : AbstractValidator<ProductsByPageDetailQuery>
{
    public ProductsByPageDetailQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .NotEmpty();
        RuleFor(x => x.PageSize)
            .NotEmpty();
    }
}