using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Products;
using FluentValidation;

namespace Consumer.Application.Products.Commands.Activate;

public sealed class ActivateProductCommandValidator : AbstractValidator<ActivateProductCommand>
{
    public ActivateProductCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.AppUserId.Value).NotEmpty();
        RuleFor(x => x.ProductId.Value).NotEmpty();
        RuleFor(x => x.ProductId)
            .MustAsync(validatorChecks.ProductExists)
            .WithMessage(x => $"{nameof(Product)} with id {x.ProductId} is not found.");
    }
}