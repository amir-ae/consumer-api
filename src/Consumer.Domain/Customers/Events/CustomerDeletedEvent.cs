using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerDeletedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerDeletedEvent(
        CustomerId customerId,
        AppUserId deletedBy,
        DateTimeOffset? deletedAt = null) : base(
        customerId)
    {
        DeletedBy = deletedBy;
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required AppUserId DeletedBy { get; init; }
    public DateTimeOffset DeletedAt { get; init; }
}