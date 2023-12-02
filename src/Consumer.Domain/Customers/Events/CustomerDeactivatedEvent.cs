using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerDeactivatedEvent : CustomerEvent
{
    public CustomerDeactivatedEvent()
    {
    }

    [SetsRequiredMembers]
    public CustomerDeactivatedEvent(
        CustomerId customerId,
        AppUserId deactivatedBy,
        DateTimeOffset? deactivatedAt = null) : base(
        customerId)
    {
        DeactivatedBy = deactivatedBy;
        DeactivatedAt = deactivatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required AppUserId DeactivatedBy { get; init; }
    public required DateTimeOffset DeactivatedAt { get; init; }
}