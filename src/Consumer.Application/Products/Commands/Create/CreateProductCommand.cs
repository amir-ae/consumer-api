using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;
using Order = Consumer.Domain.Common.Entities.Order;

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
    HashSet<Order>? Orders,
    bool? IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    AppUserId CreateBy,
    DateTimeOffset? CreateAt) : IRequest<ErrorOr<Product>>;