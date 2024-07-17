using FluentValidation;

namespace Consumer.Application.Products.Commands.Update;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.UpdateBy.Value).NotEmpty();
        RuleFor(x => x.ProductId.Value).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Brand).MaximumLength(50);
        RuleFor(x => x.Model).MaximumLength(100);
        RuleFor(x => x.DeviceType).MaximumLength(30);
        RuleFor(x => x.PanelModel).MaximumLength(50);
        RuleFor(x => x.PanelSerialNumber).MaximumLength(100);
        RuleFor(x => x.WarrantyCardNumber).MaximumLength(50);
        RuleFor(x => x.InvoiceNumber).MaximumLength(50);
        RuleFor(x => x.PurchasePrice).GreaterThan(0);
        RuleFor(x => x.DemanderFullName).MaximumLength(150);
        RuleFor(x => x.Version).GreaterThan(0);
    }
}