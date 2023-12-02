using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductModelChangedEvent : ProductEvent
{
    public ProductModelChangedEvent()
    {
    }

    [SetsRequiredMembers]
    public ProductModelChangedEvent(
        ProductId productId,
        string model,
        AppUserId modelChangedBy,
        DateTimeOffset? modelChangedAt = null) : base(
        productId)
    {
        Model = model;
        ModelChangedBy = modelChangedBy;
        ModelChangedAt = modelChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string Model { get; init; }
    public required AppUserId ModelChangedBy { get; init; }
    public required DateTimeOffset ModelChangedAt { get; init; }
}