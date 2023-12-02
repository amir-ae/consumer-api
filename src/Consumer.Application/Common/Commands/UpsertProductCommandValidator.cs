using FluentValidation;

namespace Consumer.Application.Common.Commands;

public sealed class UpsertProductCommandValidator : AbstractValidator<UpsertProductCommand>
{
    public UpsertProductCommandValidator()
    {
        RuleFor(x => x.ProductId.Value).NotEmpty();
    }
}