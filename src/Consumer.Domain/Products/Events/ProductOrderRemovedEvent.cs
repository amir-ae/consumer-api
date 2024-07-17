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
        ProductOrder order,
        HashSet<ProductOrder>? orders,
        AppUserId actor,
        DateTimeOffset? orderRemovedAt = null) : base(
        productId, actor)
    {
        Order = order;
        Orders = orders ?? new();
        OrderRemovedAt = orderRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required ProductOrder Order { get; init; }
    public required HashSet<ProductOrder> Orders { get; init; }
    public string OrdersString => string.Join(';', Orders.Select(key => $"{key.OrderId.Value},{key.CentreId.Value}"));
    public DateTimeOffset OrderRemovedAt { get; init; }
}