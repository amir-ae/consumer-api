using FluentValidation;

namespace Consumer.Application.Products.Commands.Create;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.CreateBy.Value).NotEmpty();
        RuleFor(x => x.ProductId.Value).NotEmpty();
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(35);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(35);
    }
}