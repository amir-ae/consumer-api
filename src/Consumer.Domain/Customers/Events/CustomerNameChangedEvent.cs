using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerNameChangedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerNameChangedEvent(
        CustomerId customerId,
        string firstName,
        string? middleName,
        string lastName,
        string? fullName,
        AppUserId actor,
        DateTimeOffset? nameChangedAt = null) : base(
        customerId, actor)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            FullName = fullName;
        }
        else if (string.IsNullOrWhiteSpace(middleName) && string.IsNullOrWhiteSpace(lastName))
        {
            FullName = firstName;
        }
        else
        {
            if (firstName.Length == 1) firstName = firstName.ToUpper() + '.';
            if (middleName?.Length == 1) middleName = middleName.ToUpper() + '.';
            var initials = firstName is [_, '.'] && middleName is [_, '.'];
            FullName = string.Concat(lastName, " ", firstName, initials ? "" : " ", middleName).Trim();
        }
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        NameChangedAt = nameChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string FirstName { get; init; }
    public required string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public DateTimeOffset NameChangedAt { get; init; }
}