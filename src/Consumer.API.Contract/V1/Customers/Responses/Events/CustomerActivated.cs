namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerActivated(
    DateTimeOffset ActivatedAt) : CustomerEvent;