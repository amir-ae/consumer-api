using Consumer.Application.Common.Commands;
using Consumer.Domain.Common.Entities;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Create;

public record CreateProductCommand(
    ProductId ProductId,
    string Brand,
    string Model,
    SerialId? SerialId,
    UpsertCustomerCommand? Owner,
    UpsertCustomerCommand? Dealer,
    string? DeviceType,
    string? PanelModel,
    string? PanelSerialNumber,
    string? WarrantyCardNumber,
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber,
    decimal? PurchasePrice,
    HashSet<ProductOrder>? Orders,
    bool? IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    AppUserId CreateBy,
    DateTimeOffset? CreateAt,
    bool SaveChanges = true) : IRequest<ErrorOr<Product>>;