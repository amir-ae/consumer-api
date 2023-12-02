namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerCreated(string CustomerId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string PhoneNumber,
    CustomerCity City,
    string Address,
    int Role,
    IList<string>? ProductIds,
    Guid CreatedBy,
    DateTimeOffset CreatedAt);