namespace Consumer.API.Contract.V1.Customers.Responses;

public record City(int CityId,
    string? Name,
    string? Oblast,
    string? PostalCode,
    string? PhoneCode);