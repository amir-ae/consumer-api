using FluentValidation;

namespace Consumer.Application.Customers.Queries.ByPage;


public sealed class CustomersByPageQueryValidator : AbstractValidator<CustomersByPageQuery>
{
    public CustomersByPageQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .NotEmpty();
        RuleFor(x => x.PageSize)
            .NotEmpty();
    }
}