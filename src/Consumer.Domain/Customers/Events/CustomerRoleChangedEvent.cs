using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerRoleChangedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerRoleChangedEvent(
        CustomerId customerId,
        CustomerRole role,
        AppUserId actor,
        DateTimeOffset? roleChangedAt = null) : base(
        customerId, actor)
    {
        Role = role;
        RoleChangedAt = roleChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required CustomerRole Role { get; init; }
    public DateTimeOffset RoleChangedAt { get; init; }
}