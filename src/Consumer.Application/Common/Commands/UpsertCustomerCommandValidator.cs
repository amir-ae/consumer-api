﻿using FluentValidation;

namespace Consumer.Application.Common.Commands;

public sealed class UpsertCustomerCommandValidator : AbstractValidator<UpsertCustomerCommand>
{
    public UpsertCustomerCommandValidator()
    {
        RuleFor(x => x.FirstName).MaximumLength(35);
        RuleFor(x => x.MiddleName).MaximumLength(35);
        RuleFor(x => x.LastName).MaximumLength(35);
        RuleFor(x => x.PhoneNumber).MaximumLength(35);
        RuleFor(x => x.Address).MaximumLength(255);
    }
}