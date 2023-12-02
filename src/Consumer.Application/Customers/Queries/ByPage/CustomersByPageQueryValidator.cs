using FluentValidation;

namespace Consumer.Application.Customers.Queries.ByPage;

public sealed class CustomersByPageQueryValidator : AbstractValidator<CustomersByPageQuery>
{
    public CustomersByPageQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}