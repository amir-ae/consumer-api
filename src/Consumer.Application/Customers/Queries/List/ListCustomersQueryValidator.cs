using FluentValidation;

namespace Consumer.Application.Customers.Queries.List;


public sealed class ListCustomersQueryValidator : AbstractValidator<ListCustomersQuery>
{
    public ListCustomersQueryValidator()
    {
    }
}