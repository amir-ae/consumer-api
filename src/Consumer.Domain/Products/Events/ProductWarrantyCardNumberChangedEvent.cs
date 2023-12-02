using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductWarrantyCardNumberChangedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductWarrantyCardNumberChangedEvent(
        ProductId productId,
        string? warrantyCardNumber,
        AppUserId warrantyCardNumberChangedBy,
        DateTimeOffset? warrantyCardNumberChangedAt = null) : base(
        productId)
    {
        WarrantyCardNumber = warrantyCardNumber;
        WarrantyCardNumberChangedBy = warrantyCardNumberChangedBy;
        WarrantyCardNumberChangedAt = warrantyCardNumberChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string? WarrantyCardNumber { get; init; }
    public required AppUserId WarrantyCardNumberChangedBy { get; init; }
    public DateTimeOffset WarrantyCardNumberChangedAt { get; init; }
}