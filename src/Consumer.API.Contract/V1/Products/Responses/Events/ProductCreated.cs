using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductCreated(
    string Brand,
    string Model,
    int? SerialId,
    string? OwnerId,
    string? OwnerName,
    string? DealerId,
    string? DealerName,
    string? DeviceType,
    string? PanelModel,
    string? PanelSerialNumber,
    string? WarrantyCardNumber,
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber,
    decimal? PurchasePrice,
    IList<Order> Orders,
    bool? IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    DateTimeOffset CreatedAt) : ProductEvent;