namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerDeactivated(
    DateTimeOffset DeactivatedAt) : CustomerEvent;