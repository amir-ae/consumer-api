using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerDeactivatedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerDeactivatedEvent(
        CustomerId customerId,
        AppUserId actor,
        DateTimeOffset? deactivatedAt = null) : base(
        customerId, actor)
    {
        DeactivatedAt = deactivatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset DeactivatedAt { get; init; }
}