using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerPhoneNumberChangedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerPhoneNumberChangedEvent(
        CustomerId customerId,
        PhoneNumber phoneNumber,
        AppUserId actor,
        DateTimeOffset? phoneNumberChangedAt = null) : base(
        customerId, actor)
    {
        PhoneNumber = phoneNumber;
        PhoneNumberChangedAt = phoneNumberChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required PhoneNumber PhoneNumber { get; init; }
    public DateTimeOffset PhoneNumberChangedAt { get; init; }
}