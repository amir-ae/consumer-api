using FluentValidation;

namespace Consumer.Application.Products.Queries.DetailByOrderId;

public sealed class ProductDetailByOrderIdQueryValidator : AbstractValidator<ProductDetailByOrderIdQuery>
{
    public ProductDetailByOrderIdQueryValidator()
    {
        RuleFor(x => x.OrderId.Value).NotEmpty();
    }
}