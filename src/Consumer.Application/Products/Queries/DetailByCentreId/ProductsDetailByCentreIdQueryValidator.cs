using FluentValidation;

namespace Consumer.Application.Products.Queries.DetailByCentreId;

public sealed class ProductsDetailByCentreIdQueryValidator : AbstractValidator<ProductsDetailByCentreIdQuery>
{
    public ProductsDetailByCentreIdQueryValidator()
    {
        RuleFor(x => x.CentreId.Value).NotEmpty();
    }
}