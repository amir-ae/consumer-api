namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerAddressChanged(string CustomerId,
    CustomerCity City,
    string Address,
    Guid ChangedBy,
    DateTimeOffset ChangedAt);