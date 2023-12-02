using FluentValidation;

namespace Consumer.Application.Customers.Queries.ByPageDetail;

public sealed class CustomersByPageDetailQueryValidator : AbstractValidator<CustomersByPageDetailQuery>
{
    public CustomersByPageDetailQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0);
    }
}