using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Responses;

namespace Consumer.API.Contract.V1.Products.Responses;

public sealed record ProductResponse(string ProductId,
    string Brand,
    string Model,
    int? SerialId,
    string? OwnerId,
    string? DealerId,
    IList<ProductOrder> Orders,
    DateTimeOffset CreatedAt,
    Guid CreatedBy,
    DateTimeOffset? LastModifiedAt,
    Guid? LastModifiedBy,
    bool IsActive,
    bool IsDeleted,
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
    ProductSerial? Serial,
    CustomerForListingResponse? Owner,
    CustomerForListingResponse? Dealer) : AuditableResponse(CreatedAt, 
    CreatedBy, 
    LastModifiedAt, 
    LastModifiedBy,
    IsActive, 
    IsDeleted);

public sealed record ProductOrder(string OrderId,
    Guid CentreId);

public sealed record ProductSerial(string Brand,
    string Model,
    string? Lot,
    DateTimeOffset? ProductionDate);