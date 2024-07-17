using FluentValidation;

namespace Consumer.Application.Products.Commands.Create;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.CreateBy.Value).NotEmpty();
        RuleFor(x => x.ProductId.Value).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Brand).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Model).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DeviceType).MaximumLength(30);
        RuleFor(x => x.PanelModel).MaximumLength(50);
        RuleFor(x => x.PanelSerialNumber).MaximumLength(100);
        RuleFor(x => x.WarrantyCardNumber).MaximumLength(50);
        RuleFor(x => x.InvoiceNumber).MaximumLength(50);
        RuleFor(x => x.PurchasePrice).GreaterThan(0);
        RuleFor(x => x.DemanderFullName).MaximumLength(150);
    }
}