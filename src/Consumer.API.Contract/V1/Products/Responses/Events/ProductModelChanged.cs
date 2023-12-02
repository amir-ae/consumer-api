namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductModelChanged(
    string Model,
    DateTimeOffset ModelChangedAt) : ProductEvent;