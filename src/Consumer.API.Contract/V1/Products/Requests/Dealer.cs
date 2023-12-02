using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Products.Requests;

public record Dealer
{
    [SetsRequiredMembers]
    public Dealer(
        string? dealerId,
        string? name = null,
        PhoneNumber? phoneNumber = null,
        int? cityId = null,
        string? address = null)
    {
        DealerId = dealerId;
        Name = name;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
    }
        
    public string? DealerId { get; init; }
    public string? Name { get; init; }
    public PhoneNumber? PhoneNumber { get; init; }
    public int? CityId { get; init; }
    public string? Address { get; init; }
}