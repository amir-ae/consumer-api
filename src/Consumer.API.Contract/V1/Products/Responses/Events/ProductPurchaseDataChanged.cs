namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductPurchaseDataChanged(string ProductId,
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber, 
    decimal? PurchasePrice,
    Guid DateOfPurchaseChangedBy,
    DateTimeOffset DateOfPurchaseChangedAt);