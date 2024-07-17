using Consumer.Domain.Customers.ValueObjects;
using FluentValidation;

namespace Consumer.Application.Customers.Commands.Create;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.CreateBy.Value).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.MiddleName).MaximumLength(50);
        RuleFor(x => x.LastName).MaximumLength(100)
            .NotEmpty().When(x => x.Role is null || x.Role.Value == CustomerRole.Owner);
        RuleFor(x => x.PhoneNumber.Value).NotEmpty().MaximumLength(30);
        RuleFor(x => x.PhoneNumber.CountryId.Value).MaximumLength(10);
        RuleFor(x => x.PhoneNumber.CountryCode).MaximumLength(10);
        RuleFor(x => x.PhoneNumber.Description).MaximumLength(50);
        RuleFor(x => x.CityId.Value).NotEmpty();
        RuleFor(x => x.Address).NotEmpty().MaximumLength(255);
    }
}