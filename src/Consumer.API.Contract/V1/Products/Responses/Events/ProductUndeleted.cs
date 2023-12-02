namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductUndeleted(string ProductId,
    Guid UndeletedBy,
    DateTimeOffset UndeletedAt);