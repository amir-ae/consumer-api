using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Responses;

namespace Consumer.API.Contract.V1.Customers.Responses;

public sealed record CustomerResponse : AuditableResponse
{
    public CustomerResponse()
    {
    }

    [SetsRequiredMembers]
    public CustomerResponse(
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
        DateTimeOffset createdAt,
        Guid createdBy,
        DateTimeOffset? lastModifiedAt,
        Guid? lastModifiedBy,
        bool isActive,
        bool isDeleted,
        HashSet<ProductForListingResponse>? products = null) : base(
        createdAt, 
        createdBy, 
        lastModifiedAt, 
        lastModifiedBy,
        isActive, 
        isDeleted)
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
        Products = products;
        Role = role;
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
    public HashSet<ProductForListingResponse>? Products { get; init; }
    public required int Role { get; init; }
}

public record CustomerCity
{
    public CustomerCity()
    {
    }

    [SetsRequiredMembers]
    public CustomerCity(
        int cityId,
        string? name = null,
        string? oblast = null,
        string? code = null)
    {
        CityId = cityId;
        Name = name;
        Oblast = oblast;
        Code = code;
    }
    public required int CityId { get; init; }
    public string? Name { get; init; }
    public string? Oblast { get; init; }
    public string? Code { get; init; }
}