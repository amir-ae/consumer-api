namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductModelChanged(string ProductId,
    string Model,
    Guid ModelChangedBy,
    DateTimeOffset ModelChangedAt);