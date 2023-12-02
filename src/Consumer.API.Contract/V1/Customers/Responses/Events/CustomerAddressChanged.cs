namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerAddressChanged(string CustomerId,
    City City,
    string Address,
    Guid ChangedBy,
    DateTimeOffset ChangedAt);