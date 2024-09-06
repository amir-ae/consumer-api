﻿using FluentValidation;

namespace Commerce.Application.Products.Commands.Update;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.UpdateBy).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Brand).MaximumLength(50);
        RuleFor(x => x.Model).MaximumLength(100);
        When(x => x.OwnerId is not null, () =>
        {
            RuleFor(x => x.OwnerId!.Value).MaximumLength(36);
        });
        When(x => x.DealerId is not null, () =>
        {
            RuleFor(x => x.DealerId!.Value).MaximumLength(36);
        });
        RuleFor(x => x.DeviceType).MaximumLength(30);
        RuleFor(x => x.PanelModel).MaximumLength(50);
        RuleFor(x => x.PanelSerialNumber).MaximumLength(100);
        RuleFor(x => x.WarrantyCardNumber).MaximumLength(50);
        RuleFor(x => x.InvoiceNumber).MaximumLength(50);
        RuleFor(x => x.PurchasePrice).GreaterThan(0);
        When(x => x.OrderIds is not null, () =>
        {
            RuleForEach(x => x.OrderIds).ChildRules(orderId =>
            {
                orderId.RuleFor(o => o.Value).MaximumLength(36);
            });
        });
        RuleFor(x => x.DemanderFullName).MaximumLength(150);
        RuleFor(x => x.Version).GreaterThan(0);
    }
}