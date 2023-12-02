using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerCreatedEvent : CustomerEvent
{
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
        CustomerRole? role,
        HashSet<ProductId>? productIds,
        HashSet<Order>? orders,
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
        Role = role ?? CustomerRole.Owner;
        ProductIds = productIds ?? new();
        Orders = orders ?? new();
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
    public required CustomerRole Role { get; init; }
    public required HashSet<ProductId> ProductIds { get; init; }
    public required HashSet<Order> Orders { get; init; }
    public required AppUserId CreatedBy { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}