namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerActivated(string CustomerId,
    Guid ActivatedBy,
    DateTimeOffset ActivatedAt);