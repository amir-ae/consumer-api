using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerPhoneNumberChanged
{
    public CustomerPhoneNumberChanged()
    {
    }

    [SetsRequiredMembers]
    public CustomerPhoneNumberChanged(
        string customerId,
        string phoneNumber,
        Guid changedBy,
        DateTimeOffset? changedAt = null)
    {
        CustomerId = customerId;
        PhoneNumber = phoneNumber;
        ChangedBy = changedBy;
        ChangedAt = changedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required string PhoneNumber { get; init; }
    public required Guid ChangedBy { get; init; }
    public required DateTimeOffset ChangedAt { get; init; }
}