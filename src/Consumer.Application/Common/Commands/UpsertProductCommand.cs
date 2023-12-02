using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Common.Commands;

public record UpsertProductCommand(
    ProductId ProductId,
    bool IsId = true,
    string? Brand = null,
    string? Model = null,
    SerialId? SerialId = null,
    string? DeviceType = null,
    string? PanelModel = null,
    string? PanelSerialNumber = null,
    string? WarrantyCardNumber = null,
    DateTimeOffset? DateOfPurchase = null,
    string? InvoiceNumber = null,
    decimal? PurchasePrice = null,
    HashSet<Order>? Orders = null,
    bool? IsUnrepairable = null,
    DateTimeOffset? DateOfDemandForCompensation = null,
    string? DemanderFullName = null);