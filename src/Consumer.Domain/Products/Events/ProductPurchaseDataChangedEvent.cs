using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductPurchaseDataChangedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductPurchaseDataChangedEvent(
        ProductId productId,
        DateTimeOffset? dateOfPurchase,
        string? invoiceNumber, 
        decimal? purchasePrice,
        AppUserId actor,
        DateTimeOffset? purchaseDataChangedAt = null) : base(
        productId, actor)
    {
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        PurchaseDataChangedAt = purchaseDataChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required DateTimeOffset? DateOfPurchase { get; init; }
    public required string? InvoiceNumber { get; init; }
    public required decimal? PurchasePrice { get; init; }
    public DateTimeOffset PurchaseDataChangedAt { get; init; }
}