using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Customers.Requests;

public record CreateCustomerRequest : CreateRequest
{
    [SetsRequiredMembers]
    public CreateCustomerRequest(
        Guid createBy,
        string firstName,
        string? middleName,
        string lastName,
        string phoneNumber,
        int cityId,
        string address,
        CustomerRole? role = null,
        IEnumerable<Product>? products = null,
        IEnumerable<Order>? orders = null,
        DateTimeOffset? createAt = null) : base(
        createBy,
        createAt)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        Role = role;
        Products = products;
        Orders = orders;
    }
    
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public required int CityId { get; init; }
    public required string Address { get; init; }
    public CustomerRole? Role { get; init; }
    public IEnumerable<Product>? Products { get; init; }
    public IEnumerable<Order>? Orders { get; init; }
}

