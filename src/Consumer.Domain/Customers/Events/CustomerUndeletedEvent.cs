using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerUndeletedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerUndeletedEvent(
        CustomerId customerId,
        AppUserId actor,
        DateTimeOffset? undeletedAt = null) : base(
        customerId, actor)
    {
        UndeletedAt = undeletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset UndeletedAt { get; init; }
}