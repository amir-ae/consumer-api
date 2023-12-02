using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.Entities;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductCreatedEvent : ProductEvent
{
    public ProductCreatedEvent()
    {
    }

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
        HashSet<Order>? orders,
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
        AppUserId createdBy,
        DateTimeOffset? createdAt = null) : base(
        productId)
    {
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

    public required string Brand { get; init; }
    public required string Model { get; init; }
    public SerialId? SerialId { get; init; }
    public CustomerId? OwnerId { get; init; }
    public string? OwnerName { get; init; }
    public CustomerId? DealerId { get; init; }
    public string? DealerName { get; init; }
    public HashSet<Order>? Orders { get; init; }
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
    public required AppUserId CreatedBy { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}