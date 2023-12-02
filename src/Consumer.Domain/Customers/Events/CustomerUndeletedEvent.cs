using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerUndeletedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerUndeletedEvent(
        CustomerId customerId,
        AppUserId undeletedBy,
        DateTimeOffset? undeletedAt = null) : base(
        customerId)
    {
        UndeletedBy = undeletedBy;
        UndeletedAt = undeletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required AppUserId UndeletedBy { get; init; }
    public DateTimeOffset UndeletedAt { get; init; }
}