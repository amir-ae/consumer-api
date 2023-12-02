namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerNameChanged(string CustomerId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    Guid ChangedBy,
    DateTimeOffset ChangedAt);