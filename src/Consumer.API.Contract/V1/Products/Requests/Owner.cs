using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Requests;

public record Owner
{
    [SetsRequiredMembers]
    public Owner(
        string? ownerId,
        string? firstName = null,
        string? middleName = null,
        string? lastName = null,
        string? phoneNumber = null,
        int? cityId = null,
        string? address = null)
    {
        OwnerId = ownerId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
    }
        
    public string? OwnerId { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public int? CityId { get; init; }
    public string? Address { get; init; }
}