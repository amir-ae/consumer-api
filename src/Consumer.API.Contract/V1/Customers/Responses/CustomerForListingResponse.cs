using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses;

public sealed record CustomerForListingResponse
{
    public CustomerForListingResponse()
    {
    }

    [SetsRequiredMembers]
    public CustomerForListingResponse(
        string customerId,
        string firstName,
        string? middleName,
        string lastName,
        string fullName,
        string phoneNumber,
        CustomerCity city,
        string address,
        HashSet<string> productIds,
        int role,
        DateTimeOffset createdAt)
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
        CreatedAt = createdAt;
    }

    public required string CustomerId { get; init; }
    public required string FirstName { get; init; }
    public required string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required string PhoneNumber { get; init; }
    public required CustomerCity City { get; init; }
    public required string Address { get; init; }
    public required HashSet<string> ProductIds { get; init; }
    public required int Role { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}