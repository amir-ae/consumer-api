namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductActivated(string ProductId,
    Guid ActivatedBy,
    DateTimeOffset ActivatedAt);