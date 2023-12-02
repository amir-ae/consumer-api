using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerOrderAddedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerOrderAddedEvent(
        CustomerId customerId,
        Order order,
        AppUserId actor,
        DateTimeOffset? orderAddedAt = null) : base(
        customerId, actor)
    {
        Order = order;
        OrderAddedAt = orderAddedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required Order Order { get; init; }
    public DateTimeOffset OrderAddedAt { get; init; }
}