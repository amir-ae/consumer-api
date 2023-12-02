using FluentValidation;

namespace Consumer.Application.Products.Commands.Update;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.UpdateBy.Value).NotEmpty();
        RuleFor(x => x.ProductId.Value).NotEmpty();
        RuleFor(x => x.Version).GreaterThan(0);
    }
}