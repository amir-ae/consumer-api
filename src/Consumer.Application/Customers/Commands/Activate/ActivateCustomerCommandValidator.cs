using FluentValidation;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers;

namespace Consumer.Application.Customers.Commands.Activate;

public sealed class ActivateCustomerCommandValidator : AbstractValidator<ActivateCustomerCommand>
{
    public ActivateCustomerCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.ActivateBy.Value).NotEmpty();
        RuleFor(x => x.CustomerId.Value).NotEmpty();
    }
}