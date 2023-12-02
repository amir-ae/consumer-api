using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductOrderRemoved(string ProductId,
    Order Order,
    Guid OrderRemovedBy,
    DateTimeOffset OrderRemovedAt);