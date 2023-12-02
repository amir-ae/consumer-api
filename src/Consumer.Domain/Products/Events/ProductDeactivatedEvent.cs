using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductDeactivatedEvent : ProductEvent
{
    public ProductDeactivatedEvent()
    {
    }

    [SetsRequiredMembers]
    public ProductDeactivatedEvent(
        ProductId productId,
        AppUserId deactivatedBy,
        DateTimeOffset? deactivatedAt = null) : base(
        productId)
    {
        DeactivatedBy = deactivatedBy;
        DeactivatedAt = deactivatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required AppUserId DeactivatedBy { get; init; }
    public required DateTimeOffset DeactivatedAt { get; init; }
}