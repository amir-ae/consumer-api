using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductActivatedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductActivatedEvent(
        ProductId productId,
        AppUserId actor,
        DateTimeOffset? activatedAt = null) : base(
        productId, actor)
    {
        ActivatedAt = activatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset ActivatedAt { get; init; }
}