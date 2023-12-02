namespace Consumer.API.Contract.V1.Customers.Responses;

public record PhoneNumber(
    string Value,
    string CountryId,
    string CountryCode,
    string? Description)
{
    public string FullNumber => string.Concat(CountryCode, " ", Value).Trim();
}