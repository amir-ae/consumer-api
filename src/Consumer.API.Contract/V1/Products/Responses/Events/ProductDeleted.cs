namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductDeleted(
    DateTimeOffset DeletedAt) : ProductEvent;