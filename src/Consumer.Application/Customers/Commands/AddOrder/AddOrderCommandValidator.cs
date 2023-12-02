using FluentValidation;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Application.Products.Commands.Create;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.AddOrder;

public sealed class AddOrderCommandValidator : AbstractValidator<AddOrderCommand>
{
    public AddOrderCommandValidator()
    {
        When(x => x.CustomerRole == CustomerRole.Owner, () => {
            RuleFor(x => x.Owner).NotEmpty();
        });
        When(x => x.CustomerRole == CustomerRole.Dealer, () =>
        {
            RuleFor(x => x.Dealer).NotEmpty();
        });
        When(x => x.Owner is not null, () => {
            RuleFor(x => x.Owner).SetValidator(new CreateCustomerCommandValidator()!);
        });
        When(x => x.Dealer is not null, () => {
            RuleFor(x => x.Dealer).SetValidator(new CreateCustomerCommandValidator()!);
        });
        RuleFor(x => x.Product).SetValidator(new CreateProductCommandValidator());
        RuleFor(x => x.ProductCondition).SetValidator(new CreateProductConditionCommandValidator());
    }
}