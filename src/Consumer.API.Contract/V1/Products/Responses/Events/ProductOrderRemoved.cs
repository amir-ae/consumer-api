namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductOrderRemoved(string ProductId,
    ProductOrder Order,
    Guid OrderRemovedBy,
    DateTimeOffset OrderRemovedAt);