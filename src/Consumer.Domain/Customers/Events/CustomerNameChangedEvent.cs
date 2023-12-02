using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerNameChangedEvent : CustomerEvent
{
    public CustomerNameChangedEvent()
    {
    }

    [SetsRequiredMembers]
    public CustomerNameChangedEvent(
        CustomerId customerId,
        string firstName,
        string? middleName,
        string lastName,
        string? fullName,
        AppUserId changedBy,
        DateTimeOffset? changedAt = null) : base(
        customerId)
    {
        if (!string.IsNullOrEmpty(fullName))
        {
            FullName = fullName;
        }
        else if (string.IsNullOrEmpty(middleName) && string.IsNullOrEmpty(lastName))
        {
            FullName = firstName;
        }
        else
        {
            if (firstName.Length == 1) firstName = firstName.ToUpper() + '.';
            if (middleName?.Length == 1) middleName = middleName.ToUpper() + '.';
            var initials = firstName.Length == 2 && firstName[1] == '.' && middleName?.Length == 2 && middleName[1] == '.';
            FullName = string.Concat(lastName, " ", firstName, initials ? "" : " ", middleName).Trim();
        }
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        ChangedBy = changedBy;
        ChangedAt = changedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string FirstName { get; init; }
    public required string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required AppUserId ChangedBy { get; init; }
    public required DateTimeOffset ChangedAt { get; init; }
}