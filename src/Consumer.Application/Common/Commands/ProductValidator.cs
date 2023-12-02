using FluentValidation;

namespace Consumer.Application.Common.Commands;

public sealed class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(x => x.ProductId.Value).NotEmpty();
    }
}