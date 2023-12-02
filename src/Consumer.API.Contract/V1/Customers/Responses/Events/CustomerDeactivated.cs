namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerDeactivated(string CustomerId,
    Guid DeactivatedBy,
    DateTimeOffset DeactivatedAt);