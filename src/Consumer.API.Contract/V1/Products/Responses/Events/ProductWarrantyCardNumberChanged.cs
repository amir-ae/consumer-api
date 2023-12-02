namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductWarrantyCardNumberChanged(string ProductId,
    string? WarrantyCardNumber,
    Guid WarrantyCardNumberChangedBy,
    DateTimeOffset WarrantyCardNumberChangedAt);