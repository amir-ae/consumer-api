using Consumer.API.Contract.V1.Common;
using Consumer.API.Contract.V1.Common.Responses;

namespace Consumer.API.Contract.V1.Customers.Responses;

public record CustomerForListingResponse(string Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    PhoneNumber PhoneNumber,
    City City,
    string Address,
    CustomerRole Role,
    IList<string> ProductIds,
    IList<Order> Orders,
    DateTimeOffset CreatedAt) : ForListingResponse(CreatedAt);