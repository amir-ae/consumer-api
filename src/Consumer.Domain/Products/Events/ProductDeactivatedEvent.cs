using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductDeactivatedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductDeactivatedEvent(
        ProductId productId,
        AppUserId actor,
        DateTimeOffset? deactivatedAt = null) : base(
        productId, actor)
    {
        DeactivatedAt = deactivatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset DeactivatedAt { get; init; }
}