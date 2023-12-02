namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductOrderAdded(string ProductId,
    ProductOrder Order,
    Guid OrderAddedBy,
    DateTimeOffset OrderAddedAt);