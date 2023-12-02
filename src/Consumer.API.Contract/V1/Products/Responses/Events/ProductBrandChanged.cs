namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductBrandChanged(string ProductId,
    string Brand,
    Guid BrandChangedBy,
    DateTimeOffset BrandChangedAt);