using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerAddressChanged
{
    public CustomerAddressChanged()
    {
    }

    [SetsRequiredMembers]
    public CustomerAddressChanged(
        string customerId,
        CustomerCity city,
        string address,
        Guid changedBy,
        DateTimeOffset? changedAt = null)
    {
        CustomerId = customerId;
        City = city;
        Address = address;
        ChangedBy = changedBy;
        ChangedAt = changedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required CustomerCity City { get; init; }
    public required string Address { get; init; }
    public required Guid ChangedBy { get; init; }
    public required DateTimeOffset ChangedAt { get; init; }
}