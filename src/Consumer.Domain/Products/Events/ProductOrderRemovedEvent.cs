using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.Entities;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductOrderRemovedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductOrderRemovedEvent(
        ProductId productId,
        Order order,
        AppUserId orderRemovedBy,
        DateTimeOffset? orderRemovedAt = null) : base(
        productId)
    {
        Order = order;
        OrderRemovedBy = orderRemovedBy;
        OrderRemovedAt = orderRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required Order Order { get; init; }
    public required AppUserId OrderRemovedBy { get; init; }
    public DateTimeOffset OrderRemovedAt { get; init; }
}