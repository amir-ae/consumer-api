using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductCreatedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductCreatedEvent(
        ProductId productId,
        string brand,
        string model,
        SerialId? serialId,
        CustomerId? ownerId,
        string? ownerName,
        CustomerId? dealerId,
        string? dealerName,
        string? deviceType,
        string? panelModel,
        string? panelSerialNumber,
        string? warrantyCardNumber,
        DateTimeOffset? dateOfPurchase,
        string? invoiceNumber,
        decimal? purchasePrice,
        HashSet<ProductOrder>? orders,
        bool? isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        AppUserId actor,
        DateTimeOffset? createdAt = null) : base(
        productId, actor)
    {
        Brand = brand;
        Model = model;
        SerialId = serialId;
        OwnerId = ownerId;
        OwnerName = ownerName;
        DealerId = dealerId;
        DealerName = dealerName;
        DeviceType = deviceType;
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        WarrantyCardNumber = warrantyCardNumber;
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        Orders = orders ?? new();
        IsUnrepairable = isUnrepairable ?? false;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }

    public required string Brand { get; init; }
    public required string Model { get; init; }
    public SerialId? SerialId { get; init; }
    public int? SerialIdValue => SerialId?.Value;
    public CustomerId? OwnerId { get; init; }
    public string? OwnerName { get; init; }
    public CustomerId? DealerId { get; init; }
    public string? DealerName { get; init; }
    public string? DeviceType { get; init; }
    public string? PanelModel { get; init; }
    public string? PanelSerialNumber { get; init; }
    public string? WarrantyCardNumber { get; init; }
    public DateTimeOffset? DateOfPurchase { get; init; }
    public string? InvoiceNumber { get; init; }
    public decimal? PurchasePrice { get; init; }
    public required HashSet<ProductOrder> Orders { get; init; }
    public string OrdersString => string.Join(';', Orders.Select(key => $"{key.OrderId.Value},{key.CentreId.Value}"));
    public bool IsUnrepairable { get; set; }
    public DateTimeOffset? DateOfDemandForCompensation { get; set; }
    public string? DemanderFullName { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
}