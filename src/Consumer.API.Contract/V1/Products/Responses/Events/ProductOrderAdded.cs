using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductOrderAdded(
    Order Order,
    IList<Order> Orders,
    DateTimeOffset OrderAddedAt) : ProductEvent;