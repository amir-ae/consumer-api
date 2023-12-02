using FluentValidation;

namespace Consumer.Application.Customers.Queries.All;


public sealed class AllCustomersQueryValidator : AbstractValidator<AllCustomersQuery>
{
    public AllCustomersQueryValidator()
    {
    }
}