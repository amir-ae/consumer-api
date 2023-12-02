namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerProductRemoved(
    string ProductId,
    DateTimeOffset ProductRemovedAt) : CustomerEvent;