using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerDeletedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerDeletedEvent(
        CustomerId customerId,
        AppUserId actor,
        DateTimeOffset? deletedAt = null) : base(
        customerId, actor)
    {
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset DeletedAt { get; init; }
}