using FluentValidation;

namespace Consumer.Application.Products.Queries.All;


public sealed class AllProductsQueryValidator : AbstractValidator<AllProductsQuery>
{
    public AllProductsQueryValidator()
    {
    }
}