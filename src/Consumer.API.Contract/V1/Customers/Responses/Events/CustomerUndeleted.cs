namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerUndeleted(string CustomerId,
    Guid UndeletedBy,
    DateTimeOffset UndeletedAt);