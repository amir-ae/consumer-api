namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerDeactivated(string CustomerId,
    Guid DeactivatedBy,
    DateTimeOffset DeactivatedAt);