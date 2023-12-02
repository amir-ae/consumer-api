using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Products;
using FluentValidation;

namespace Consumer.Application.Products.Commands.Delete;

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.DeleteBy.Value).NotEmpty();
        RuleFor(x => x.ProductId.Value).NotEmpty();
    }
}