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
        AppUserId actor,
        DateTimeOffset? addressChangedAt = null) : base(
        customerId, actor)
    {
        CityId = cityId;
        Address = address;
        AddressChangedAt = addressChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required CityId CityId { get; init; }
    public required string Address { get; init; }
    public DateTimeOffset AddressChangedAt { get; init; }
}