namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductDealerChanged(
    string? DealerId,
    string? DealerName,
    DateTimeOffset DealerChangedAt) : ProductEvent;