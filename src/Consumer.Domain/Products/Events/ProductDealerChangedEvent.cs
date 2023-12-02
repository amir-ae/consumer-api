using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductDealerChangedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductDealerChangedEvent(
        ProductId productId,
        CustomerId? dealerId,
        string? dealerName,
        AppUserId actor,
        DateTimeOffset? dealerChangedAt = null) : base(
        productId, actor)
    {
        DealerId = dealerId;
        DealerName = dealerName;
        DealerChangedAt = dealerChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required CustomerId? DealerId { get; init; }
    public required string? DealerName { get; init; }
    public DateTimeOffset DealerChangedAt { get; init; }
}