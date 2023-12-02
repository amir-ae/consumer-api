namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductUndeleted(
    DateTimeOffset UndeletedAt) : ProductEvent;