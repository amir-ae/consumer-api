using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerOrderRemoved(
    Order Order,
    DateTimeOffset OrderRemovedAt) : CustomerEvent;