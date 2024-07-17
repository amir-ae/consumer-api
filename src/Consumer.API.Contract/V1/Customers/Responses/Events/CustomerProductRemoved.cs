namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerProductRemoved(
    string ProductId,
    IList<string> ProductIds,
    DateTimeOffset ProductRemovedAt) : CustomerEvent;