using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerRoleChangedEvent : CustomerEvent
{
    public CustomerRoleChangedEvent()
    {
    }

    [SetsRequiredMembers]
    public CustomerRoleChangedEvent(
        CustomerId customerId,
        CustomerRole role,
        AppUserId changedBy,
        DateTimeOffset? changedAt = null) : base(
        customerId)
    {
        Role = role;
        ChangedBy = changedBy;
        ChangedAt = changedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required CustomerRole Role { get; init; }
    public required AppUserId ChangedBy { get; init; }
    public required DateTimeOffset ChangedAt { get; init; }
}