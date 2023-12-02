namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductDeactivated(
    DateTimeOffset DeactivatedAt) : ProductEvent;