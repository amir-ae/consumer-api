namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerProductAdded(
    string ProductId,
    IList<string> ProductIds,
    DateTimeOffset ProductAddedAt) : CustomerEvent;