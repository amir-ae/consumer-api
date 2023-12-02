using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerAddressChangedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerAddressChangedEvent(
        CustomerId customerId,
        CityId cityId,
        string address,
        AppUserId changedBy,
        DateTimeOffset? changedAt = null) : base(
        customerId)
    {
        CityId = cityId;
        Address = address;
        ChangedBy = changedBy;
        ChangedAt = changedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required CityId CityId { get; init; }
    public required string Address { get; init; }
    public required AppUserId ChangedBy { get; init; }
    public DateTimeOffset ChangedAt { get; init; }
}