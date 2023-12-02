using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerCreated
{
    public CustomerCreated()
    {
    }

    [SetsRequiredMembers]
    public CustomerCreated(
        string customerId,
        string firstName,
        string? middleName,
        string lastName,
        string fullName,
        string phoneNumber,
        CustomerCity city,
        string address,
        List<string>? productIds,
        int role,
        Guid createdBy,
        DateTimeOffset? createdAt = null)
    {
        CustomerId = customerId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        City = city;
        Address = address;
        ProductIds = productIds;
        Role = role;
        CreatedBy = createdBy;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required string FirstName { get; init; }
    public required string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required string PhoneNumber { get; init; }
    public required CustomerCity City { get; init; }
    public required string Address { get; init; }
    public required List<string>? ProductIds { get; init; }
    public required int Role { get; init; }
    public required Guid CreatedBy { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}