using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers;
using FluentValidation;

namespace Consumer.Application.Customers.Commands.Delete;

public sealed class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator(IValidatorChecks validatorChecks)
    {
        RuleFor(x => x.DeleteBy.Value).NotEmpty();
        RuleFor(x => x.CustomerId.Value).NotEmpty();
    }
}