using FluentValidation;

namespace Consumer.Application.Customers.Queries.ByPageDetail;


public sealed class CustomersByPageDetailQueryValidator : AbstractValidator<CustomersByPageDetailQuery>
{
    public CustomersByPageDetailQueryValidator()
    {
        RuleFor(x => x.PageIndex)
            .NotEmpty();
        RuleFor(x => x.PageSize)
            .NotEmpty();
    }
}