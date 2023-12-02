using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductModelChangedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductModelChangedEvent(
        ProductId productId,
        string model,
        AppUserId actor,
        DateTimeOffset? modelChangedAt = null) : base(
        productId, actor)
    {
        Model = model;
        ModelChangedAt = modelChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string Model { get; init; }
    public DateTimeOffset ModelChangedAt { get; init; }
}