using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductUndeletedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductUndeletedEvent(
        ProductId productId,
        AppUserId actor,
        DateTimeOffset? undeletedAt = null) : base(
        productId, actor)
    {
        UndeletedAt = undeletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset UndeletedAt { get; init; }
}