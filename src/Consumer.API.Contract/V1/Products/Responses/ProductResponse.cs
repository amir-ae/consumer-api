using Consumer.API.Contract.V1.Common;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Responses;

namespace Consumer.API.Contract.V1.Products.Responses;

public record ProductResponse(string Id,
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
    Serial? Serial,
    CustomerForListingResponse? Owner,
    CustomerForListingResponse? Dealer,
    int Version,
    DateTimeOffset CreatedAt,
    Guid CreatedBy,
    DateTimeOffset? LastModifiedAt,
    Guid? LastModifiedBy,
    bool IsActive,
    bool IsDeleted) : AuditableResponse(Version,
    CreatedAt, 
    CreatedBy, 
    LastModifiedAt, 
    LastModifiedBy,
    IsActive, 
    IsDeleted);