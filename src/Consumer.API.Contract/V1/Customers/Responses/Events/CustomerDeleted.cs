namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerDeleted(string CustomerId,
    Guid DeletedBy,
    DateTimeOffset DeletedAt);