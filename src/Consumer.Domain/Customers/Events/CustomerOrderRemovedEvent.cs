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
        Order order,
        AppUserId actor,
        DateTimeOffset? orderRemovedAt = null) : base(
        customerId, actor)
    {
        Order = order;
        OrderRemovedAt = orderRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required Order Order { get; init; }
    public DateTimeOffset OrderRemovedAt { get; init; }
}