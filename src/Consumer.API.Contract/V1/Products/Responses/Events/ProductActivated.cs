namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductActivated(
    DateTimeOffset ActivatedAt) : ProductEvent;