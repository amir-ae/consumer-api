using FluentValidation;

namespace Consumer.Application.Products.Queries.List;


public sealed class ListProductsQueryValidator : AbstractValidator<ListProductsQuery>
{
    public ListProductsQueryValidator()
    {
    }
}