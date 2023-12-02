using FluentValidation;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers;

namespace Consumer.Application.Customers.Commands.Deactivate;

public sealed class DeactivateCustomerCommandValidator : AbstractValidator<DeactivateCustomerCommand>
{
    public DeactivateCustomerCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.DeactivateBy.Value).NotEmpty();
        RuleFor(x => x.CustomerId.Value).NotEmpty();
    }
}