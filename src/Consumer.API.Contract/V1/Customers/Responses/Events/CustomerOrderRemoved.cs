using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerOrderRemoved(
    Order Order,
    IList<Order> Orders,
    DateTimeOffset OrderRemovedAt) : CustomerEvent;