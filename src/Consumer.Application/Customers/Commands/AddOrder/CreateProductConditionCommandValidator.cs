using FluentValidation;

namespace Consumer.Application.Customers.Commands.AddOrder;

public sealed class CreateProductConditionCommandValidator : AbstractValidator<CreateProductConditionCommand>
{
    public CreateProductConditionCommandValidator()
    {
        RuleFor(x => x.CentreId.Value).NotEmpty();
        RuleFor(x => x.Completeness).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Appearance).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Malfunction).NotEmpty().MaximumLength(255);
    }
}