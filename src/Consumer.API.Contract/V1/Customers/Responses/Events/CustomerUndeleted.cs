namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerUndeleted(string CustomerId,
    Guid UndeletedBy,
    DateTimeOffset UndeletedAt);