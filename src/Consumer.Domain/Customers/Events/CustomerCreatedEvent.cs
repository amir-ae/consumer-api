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
        PhoneNumber phoneNumber,
        CityId cityId,
        string address,
        CustomerRole? role,
        HashSet<ProductId>? productIds,
        HashSet<CustomerOrder>? orders,
        AppUserId actor,
        DateTimeOffset? createdAt = null) : base(
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
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        Role = role ?? CustomerRole.Owner;
        ProductIds = productIds ?? new();
        Orders = orders ?? new();
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string FirstName { get; init; }
    public required string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required PhoneNumber PhoneNumber { get; init; }
    public required CityId CityId { get; init; }
    public required string Address { get; init; }
    public required CustomerRole Role { get; init; }
    public required HashSet<ProductId> ProductIds { get; init; }
    public string ProductIdsString => string.Join(',', ProductIds.Select(p => p.Value));
    public required HashSet<CustomerOrder> Orders { get; init; }
    public string OrdersString => string.Join(';', Orders.Select(key => $"{key.OrderId.Value},{key.CentreId.Value}"));
    public DateTimeOffset CreatedAt { get; init; }
}