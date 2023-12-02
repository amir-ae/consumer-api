using FluentValidation;

namespace Consumer.Application.Customers.Queries.DetailById;


public sealed class CustomerDetailByIdQueryValidator : AbstractValidator<CustomerDetailByIdQuery>
{
    public CustomerDetailByIdQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}