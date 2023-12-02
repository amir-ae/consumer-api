using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductOrderAddedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductOrderAddedEvent(
        ProductId productId,
        Order order,
        AppUserId actor,
        DateTimeOffset? orderAddedAt = null) : base(
        productId, actor)
    {
        Order = order;
        OrderAddedAt = orderAddedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required Order Order { get; init; }
    public DateTimeOffset OrderAddedAt { get; init; }
}