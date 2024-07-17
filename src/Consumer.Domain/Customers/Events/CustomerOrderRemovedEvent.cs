using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers.Events;
public sealed record CustomerOrderRemovedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerOrderRemovedEvent(
        CustomerId customerId,
        CustomerOrder order,
        HashSet<CustomerOrder>? orders,
        AppUserId actor,
        DateTimeOffset? orderRemovedAt = null) : base(
        customerId, actor)
    {
        Order = order;
        Orders = orders ?? new();
        OrderRemovedAt = orderRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required CustomerOrder Order { get; init; }
    public required HashSet<CustomerOrder> Orders { get; init; }
    public string OrdersString => string.Join(';', Orders.Select(key => $"{key.OrderId.Value},{key.CentreId.Value}"));
    public DateTimeOffset OrderRemovedAt { get; init; }
}