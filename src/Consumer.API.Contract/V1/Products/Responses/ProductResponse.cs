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
    DateTimeOffset CreatedAt,
    Guid CreatedBy,
    DateTimeOffset? LastModifiedAt,
    Guid? LastModifiedBy,
    int Version,
    bool IsActive,
    bool IsDeleted) : AuditableResponse(CreatedAt, 
    CreatedBy, 
    LastModifiedAt, 
    LastModifiedBy,
    Version,
    IsActive, 
    IsDeleted);