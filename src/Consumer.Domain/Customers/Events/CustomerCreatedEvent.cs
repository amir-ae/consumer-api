using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerCreatedEvent : CustomerEvent
{
    public CustomerCreatedEvent()
    {
    }

    [SetsRequiredMembers]
    public CustomerCreatedEvent(
        CustomerId customerId,
        string firstName,
        string? middleName,
        string lastName,
        string? fullName,
        string phoneNumber,
        CityId cityId,
        string address,
        HashSet<ProductId>? productIds,
        CustomerRole? role,
        AppUserId createdBy,
        DateTimeOffset? createdAt = null) : base(
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
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        ProductIds = productIds;
        Role = role ?? CustomerRole.Owner;
        CreatedBy = createdBy;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string FirstName { get; init; }
    public required string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required string PhoneNumber { get; init; }
    public required CityId CityId { get; init; }
    public required string Address { get; init; }
    public required HashSet<ProductId>? ProductIds { get; init; }
    public required CustomerRole Role { get; init; }
    public required AppUserId CreatedBy { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}