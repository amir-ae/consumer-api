namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductDealerChanged(string ProductId,
    string? DealerId,
    string? DealerName,
    Guid DealerChangedBy,
    DateTimeOffset DealerChangedAt);