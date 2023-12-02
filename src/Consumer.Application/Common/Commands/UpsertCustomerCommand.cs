using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Common.Commands;

public record UpsertCustomerCommand(
    CustomerId? CustomerId,
    bool IsId = true,
    string? FirstName = null,
    string? MiddleName = null,
    string? LastName = null,
    PhoneNumber? PhoneNumber = null,
    CityId? CityId = null,
    string? Address = null,
    CustomerRole? Role = null);