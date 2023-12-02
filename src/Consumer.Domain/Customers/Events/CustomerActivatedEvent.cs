using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerActivatedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerActivatedEvent(
        CustomerId customerId,
        AppUserId activatedBy,
        DateTimeOffset? activatedAt = null) : base(
        customerId)
    {
        ActivatedBy = activatedBy;
        ActivatedAt = activatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required AppUserId ActivatedBy { get; init; }
    public DateTimeOffset ActivatedAt { get; init; }
}