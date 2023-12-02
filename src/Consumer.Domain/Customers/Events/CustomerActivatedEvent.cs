using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerActivatedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerActivatedEvent(
        CustomerId customerId,
        AppUserId actor,
        DateTimeOffset? activatedAt = null) : base(
        customerId, actor)
    {
        ActivatedAt = activatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset ActivatedAt { get; init; }
}