using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Customers.Requests;

public sealed record PostCustomerRequest : PostRequest
{
    public PostCustomerRequest()
    {
    }

    [SetsRequiredMembers]
    public PostCustomerRequest(
        Guid postBy,
        string firstName,
        string? middleName,
        string lastName,
        string phoneNumber,
        int cityId,
        string address,
        List<string>? productIds = null,
        int? role = null,
        DateTimeOffset? postAt = null) : base(
        postBy,
        postAt)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        ProductIds = productIds;
        Role = role;
    }
    
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public required int CityId { get; init; }
    public required string Address { get; init; }
    public List<string>? ProductIds { get; init; }
    public required int? Role { get; init; }
}