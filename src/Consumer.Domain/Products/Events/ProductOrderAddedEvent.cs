using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.Entities;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductOrderAddedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductOrderAddedEvent(
        ProductId productId,
        Order order,
        AppUserId orderAddedBy,
        DateTimeOffset? orderAddedAt = null) : base(
        productId)
    {
        Order = order;
        OrderAddedBy = orderAddedBy;
        OrderAddedAt = orderAddedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required Order Order { get; init; }
    public required AppUserId OrderAddedBy { get; init; }
    public DateTimeOffset OrderAddedAt { get; init; }
}