using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerRoleChanged(string CustomerId,
    CustomerRole Role,
    Guid ChangedBy,
    DateTimeOffset ChangedAt);