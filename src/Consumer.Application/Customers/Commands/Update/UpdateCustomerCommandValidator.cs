using FluentValidation;

namespace Consumer.Application.Customers.Commands.Update;

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.UpdateBy.Value).NotEmpty();
        RuleFor(x => x.CustomerId.Value).NotEmpty().MaximumLength(36);
        RuleFor(x => x.FirstName).MaximumLength(50);
        RuleFor(x => x.MiddleName).MaximumLength(50);
        RuleFor(x => x.LastName).MaximumLength(100);
        When(x => x.PhoneNumber is not null, () =>
        {
            RuleFor(x => x.PhoneNumber!.Value).MaximumLength(30);
            RuleFor(x => x.PhoneNumber!.CountryId.Value).MaximumLength(10);
            RuleFor(x => x.PhoneNumber!.CountryCode).MaximumLength(10);
            RuleFor(x => x.PhoneNumber!.Description).MaximumLength(50);
        });
        RuleFor(x => x.Address).MaximumLength(255);
        RuleFor(x => x.Version).GreaterThan(0);
    }
}