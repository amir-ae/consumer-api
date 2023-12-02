using Consumer.Application.Common.Interfaces.Persistence;
using FluentValidation;

namespace Consumer.Application.Customers.Commands.Update;

public sealed class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;
    
    public UpdateCustomerCommandValidator(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
        RuleFor(x => x.AppUserId.Value).NotEmpty();
        RuleFor(x => x.CustomerId.Value).NotEmpty();
        RuleFor(x => x.FirstName).MaximumLength(35);
        RuleFor(x => x.MiddleName).MaximumLength(35);
        RuleFor(x => x.LastName).MaximumLength(35);
        RuleFor(x => x.PhoneNumber).MaximumLength(35);
        RuleFor(x => x.Address).MaximumLength(255);
    }
}