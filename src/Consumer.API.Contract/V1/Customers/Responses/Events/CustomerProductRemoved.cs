namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerProductRemoved(string CustomerId,
    string ProductId,
    Guid ProductRemovedBy,
    DateTimeOffset ProductRemovedAt);