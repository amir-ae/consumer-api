using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Requests;

public record Dealer
{
    [SetsRequiredMembers]
    public Dealer(
        string? dealerId,
        string? name = null,
        string? phoneNumber = null,
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
    public string? PhoneNumber { get; init; }
    public int? CityId { get; init; }
    public string? Address { get; init; }
}