using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Customers.Requests;

public sealed record PatchCustomerRequest : PatchRequest
{
    [SetsRequiredMembers]
    public PatchCustomerRequest(
        Guid patchBy,
        string? firstName = null,
        string? middleName = null,
        string? lastName = null,
        string? phoneNumber = null,
        int? cityId = null,
        string? address = null,
        int? role = null,
        IEnumerable<CustomerProduct>? products = null,
        DateTimeOffset? patchAt = null) : base(
        patchBy,
        patchAt)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        Role = role;
        Products = products;
    }
    
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public int? CityId { get; init; }
    public string? Address { get; init; }
    public int? Role { get; init; }
    public IEnumerable<CustomerProduct>? Products { get; init; }
}