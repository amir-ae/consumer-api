using Consumer.Application.Customers.Queries.ById;
using FluentValidation;

namespace Consumer.Application.Products.Queries.EventsById;


public sealed class ProductEventsByIdQueryValidator : AbstractValidator<ProductEventsByIdQuery>
{
    public ProductEventsByIdQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}