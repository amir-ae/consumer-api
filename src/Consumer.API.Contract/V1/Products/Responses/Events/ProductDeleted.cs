namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductDeleted(string ProductId,
    Guid DeletedBy,
    DateTimeOffset DeletedAt);