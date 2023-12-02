namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerActivated(string CustomerId,
    Guid ActivatedBy,
    DateTimeOffset ActivatedAt);