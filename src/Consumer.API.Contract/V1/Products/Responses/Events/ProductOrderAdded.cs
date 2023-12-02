using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductOrderAdded(string ProductId,
    Order Order,
    Guid OrderAddedBy,
    DateTimeOffset OrderAddedAt);