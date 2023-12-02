using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerNameChanged
{
    public CustomerNameChanged()
    {
    }

    [SetsRequiredMembers]
    public CustomerNameChanged(
        string customerId,
        string firstName,
        string? middleName,
        string lastName,
        string fullName,
        Guid changedBy,
        DateTimeOffset? changedAt = null)
    {
        CustomerId = customerId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        FullName = fullName;
        ChangedBy = changedBy;
        ChangedAt = changedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required string FirstName { get; init; }
    public required string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required Guid ChangedBy { get; init; }
    public required DateTimeOffset ChangedAt { get; init; }
}