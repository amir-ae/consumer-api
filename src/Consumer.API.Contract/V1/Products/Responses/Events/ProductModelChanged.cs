namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductModelChanged(string ProductId,
    string Model,
    Guid ModelChangedBy,
    DateTimeOffset ModelChangedAt);