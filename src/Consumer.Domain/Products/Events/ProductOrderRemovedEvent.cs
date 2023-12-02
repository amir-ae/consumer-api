using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductOrderRemovedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductOrderRemovedEvent(
        ProductId productId,
        Order order,
        AppUserId actor,
        DateTimeOffset? orderRemovedAt = null) : base(
        productId, actor)
    {
        Order = order;
        OrderRemovedAt = orderRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required Order Order { get; init; }
    public DateTimeOffset OrderRemovedAt { get; init; }
}