using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerOrderAdded(
    Order Order,
    DateTimeOffset OrderAddedAt) : CustomerEvent;