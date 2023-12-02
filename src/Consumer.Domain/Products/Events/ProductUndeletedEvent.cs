using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductUndeletedEvent : ProductEvent
{
    public ProductUndeletedEvent()
    {
    }

    [SetsRequiredMembers]
    public ProductUndeletedEvent(
        ProductId productId,
        AppUserId undeletedBy,
        DateTimeOffset? undeletedAt = null) : base(
        productId)
    {
        UndeletedBy = undeletedBy;
        UndeletedAt = undeletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required AppUserId UndeletedBy { get; init; }
    public required DateTimeOffset UndeletedAt { get; init; }
}