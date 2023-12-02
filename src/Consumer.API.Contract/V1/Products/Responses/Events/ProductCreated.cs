using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductCreated
{
    public ProductCreated()
    {
    }

    [SetsRequiredMembers]
    public ProductCreated(
        string productId,
        string brand,
        string model,
        int? serialId,
        string? ownerId,
        string? ownerName,
        string? dealerId,
        string? dealerName,
        HashSet<ProductOrder>? orders,
        string? deviceType,
        string? panelModel,
        string? panelSerialNumber,
        string? warrantyCardNumber,
        DateTimeOffset? dateOfPurchase,
        string? invoiceNumber,
        decimal? purchasePrice,
        bool? isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        Guid createdBy,
        DateTimeOffset? createdAt = null)
    {
        ProductId = productId;
        Brand = brand;
        Model = model;
        SerialId = serialId;
        OwnerId = ownerId;
        OwnerName = ownerName;
        DealerId = dealerId;
        DealerName = dealerName;
        Orders = orders;
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
        CreatedBy = createdBy;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public required string ProductId { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public int? SerialId { get; init; }
    public string? OwnerId { get; init; }
    public string? OwnerName { get; init; }
    public string? DealerId { get; init; }
    public string? DealerName { get; init; }
    public HashSet<ProductOrder>? Orders { get; init; }
    public string? DeviceType { get; init; }
    public string? PanelModel { get; init; }
    public string? PanelSerialNumber { get; init; }
    public string? WarrantyCardNumber { get; init; }
    public DateTimeOffset? DateOfPurchase { get; init; }
    public string? InvoiceNumber { get; init; }
    public decimal? PurchasePrice { get; init; }
    public bool? IsUnrepairable { get; set; }
    public DateTimeOffset? DateOfDemandForCompensation { get; set; }
    public string? DemanderFullName { get; set; }
    public required Guid CreatedBy { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}