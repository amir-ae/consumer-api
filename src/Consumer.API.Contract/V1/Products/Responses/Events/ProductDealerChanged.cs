namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductDealerChanged(string ProductId,
    string? DealerId,
    string? DealerName,
    Guid DealerChangedBy,
    DateTimeOffset DealerChangedAt);