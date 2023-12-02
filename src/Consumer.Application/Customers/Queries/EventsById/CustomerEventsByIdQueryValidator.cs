using Consumer.Application.Customers.Queries.ById;
using FluentValidation;

namespace Consumer.Application.Customers.Queries.EventsById;


public sealed class CustomerEventsByIdQueryValidator : AbstractValidator<CustomerByIdQuery>
{
    public CustomerEventsByIdQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}