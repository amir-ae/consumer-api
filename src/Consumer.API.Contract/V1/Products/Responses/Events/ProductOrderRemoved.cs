using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductOrderRemoved(
    Order Order,
    DateTimeOffset OrderRemovedAt) : ProductEvent;