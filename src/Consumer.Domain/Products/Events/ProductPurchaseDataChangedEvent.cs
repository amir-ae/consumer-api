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
        AppUserId dateOfPurchaseChangedBy,
        DateTimeOffset? dateOfPurchaseChangedAt = null) : base(
        productId)
    {
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        DateOfPurchaseChangedBy = dateOfPurchaseChangedBy;
        DateOfPurchaseChangedAt = dateOfPurchaseChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required DateTimeOffset? DateOfPurchase { get; init; }
    public required string? InvoiceNumber { get; init; }
    public required decimal? PurchasePrice { get; init; }
    public required AppUserId DateOfPurchaseChangedBy { get; init; }
    public DateTimeOffset DateOfPurchaseChangedAt { get; init; }
}