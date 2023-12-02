namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductPurchaseDataChanged(string ProductId,
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber, 
    decimal? PurchasePrice,
    Guid DateOfPurchaseChangedBy,
    DateTimeOffset DateOfPurchaseChangedAt);