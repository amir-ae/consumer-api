namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerRoleChanged(string CustomerId,
    int Role,
    Guid ChangedBy,
    DateTimeOffset ChangedAt);