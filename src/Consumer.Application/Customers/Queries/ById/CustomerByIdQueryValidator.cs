using FluentValidation;

namespace Consumer.Application.Customers.Queries.ById;


public sealed class CustomerByIdQueryValidator : AbstractValidator<CustomerByIdQuery>
{
    public CustomerByIdQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}