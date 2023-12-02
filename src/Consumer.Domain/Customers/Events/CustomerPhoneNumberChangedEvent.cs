using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerPhoneNumberChangedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerPhoneNumberChangedEvent(
        CustomerId customerId,
        string phoneNumber,
        AppUserId changedBy,
        DateTimeOffset? changedAt = null) : base(
        customerId)
    {
        PhoneNumber = phoneNumber;
        ChangedBy = changedBy;
        ChangedAt = changedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string PhoneNumber { get; init; }
    public required AppUserId ChangedBy { get; init; }
    public DateTimeOffset ChangedAt { get; init; }
}