using FluentValidation;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers;

namespace Consumer.Application.Customers.Commands.Deactivate;

public sealed class DeactivateCustomerCommandValidator : AbstractValidator<DeactivateCustomerCommand>
{
    public DeactivateCustomerCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.AppUserId.Value).NotEmpty();
        RuleFor(x => x.CustomerId.Value).NotEmpty();
        RuleFor(x => x.CustomerId)
            .MustAsync(validatorChecks.CustomerExists)
            .WithMessage(x => $"{nameof(Customer)} with id {x.CustomerId} is not found.");
    }
}