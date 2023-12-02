namespace Consumer.API.Contract.V1.Customers.Responses;

public sealed record CustomerForListingResponse(string CustomerId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string PhoneNumber,
    CustomerCity City,
    string Address,
    int Role,
    IList<string> ProductIds,
    DateTimeOffset CreatedAt);