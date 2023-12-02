namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerProductAdded(string CustomerId,
    string ProductId,
    Guid ProductAddedBy,
    DateTimeOffset ProductAddedAt);