using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductActivatedEvent : ProductEvent
{
    public ProductActivatedEvent()
    {
    }

    [SetsRequiredMembers]
    public ProductActivatedEvent(
        ProductId productId,
        AppUserId activatedBy,
        DateTimeOffset? activatedAt = null) : base(
        productId)
    {
        ActivatedBy = activatedBy;
        ActivatedAt = activatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required AppUserId ActivatedBy { get; init; }
    public required DateTimeOffset ActivatedAt { get; init; }
}