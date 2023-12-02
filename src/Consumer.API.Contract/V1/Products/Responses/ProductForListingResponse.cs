using Consumer.API.Contract.V1.Common;
using Consumer.API.Contract.V1.Common.Responses;

namespace Consumer.API.Contract.V1.Products.Responses;

public record ProductForListingResponse(string Id,
    string Brand,
    string Model,
    int? SerialId,
    string? OwnerId,
    string? DealerId,
    IList<Order> Orders,
    string? DeviceType,
    string? PanelModel,
    string? PanelSerialNumber,
    string? WarrantyCardNumber,
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber,
    decimal? PurchasePrice,
    bool? IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    DateTimeOffset CreatedAt) : ForListingResponse(CreatedAt);