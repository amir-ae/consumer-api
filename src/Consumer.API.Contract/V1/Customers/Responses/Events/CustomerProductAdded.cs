namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerProductAdded(
    string ProductId,
    DateTimeOffset ProductAddedAt) : CustomerEvent;