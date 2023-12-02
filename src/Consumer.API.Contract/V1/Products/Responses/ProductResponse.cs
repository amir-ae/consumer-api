using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Responses;

namespace Consumer.API.Contract.V1.Products.Responses;

public sealed record ProductResponse : AuditableResponse
{
    public ProductResponse()
    {
    }
    
    [SetsRequiredMembers]
    public ProductResponse(
        string productId,
        string brand,
        string model,
        int? serialId,
        string? ownerId,
        string? dealerId,
        List<ProductOrder> orders,
        DateTimeOffset createdAt,
        Guid createdBy,
        DateTimeOffset? lastModifiedAt,
        Guid? lastModifiedBy,
        bool isActive,
        bool isDeleted,
        string? deviceType = null,
        string? panelModel = null,
        string? panelSerialNumber = null,
        string? warrantyCardNumber = null,
        DateTimeOffset? dateOfPurchase = null,
        string? invoiceNumber = null,
        decimal? purchasePrice = null,
        bool? isUnrepairable = null,
        DateTimeOffset? dateOfDemandForCompensation = null,
        string? demanderFullName = null,
        ProductSerial? serial = null,
        CustomerForListingResponse? owner = null,
        CustomerForListingResponse? dealer = null) : base(
        createdAt, 
        createdBy, 
        lastModifiedAt, 
        lastModifiedBy,
        isActive, 
        isDeleted)
    {
        ProductId = productId;
        Brand = brand;
        Model = model;
        SerialId = serialId;
        Serial = serial;
        Orders = orders;
        OwnerId = ownerId;
        Owner = owner;
        DealerId = dealerId;
        Dealer = dealer;
        DeviceType = deviceType;
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        WarrantyCardNumber = warrantyCardNumber;
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        IsUnrepairable = isUnrepairable;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
    }
    public required string ProductId { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public int? SerialId { get; init; }
    public ProductSerial? Serial { get; set; }
    public required List<ProductOrder> Orders { get; init; }
    public string? OwnerId { get; init; }
    public CustomerForListingResponse? Owner { get; set; }
    public string? DealerId { get; init; }
    public CustomerForListingResponse? Dealer { get; set; }
    public string? DeviceType { get; init; }
    public string? PanelModel { get; init; }
    public string? PanelSerialNumber { get; init; }
    public string? WarrantyCardNumber { get; init; }
    public DateTimeOffset? DateOfPurchase { get; init; }
    public string? InvoiceNumber { get; init; }
    public decimal? PurchasePrice { get; init; }
    public bool? IsUnrepairable { get; init; }
    public DateTimeOffset? DateOfDemandForCompensation { get; init; }
    public string? DemanderFullName { get; init; }
}

public sealed record ProductOrder
{
    public ProductOrder()
    {
    }

    [SetsRequiredMembers]
    public ProductOrder(
        string orderId,
        Guid centreId)
    {
        OrderId = orderId;
        CentreId = centreId;
    }

    public required string OrderId { get; init; }
    public required Guid CentreId { get; init; }
}

public record ProductSerial
{
    public ProductSerial()
    {
    }

    [SetsRequiredMembers]
    public ProductSerial(
        string brand,
        string model,
        string? lot,
        DateTimeOffset? productionDate)
    {
        Brand = brand;
        Model = model;
        Lot = lot;
        ProductionDate = productionDate;
    }
    
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public string? Lot { get; init; }
    public required DateTimeOffset? ProductionDate { get; init; }
}