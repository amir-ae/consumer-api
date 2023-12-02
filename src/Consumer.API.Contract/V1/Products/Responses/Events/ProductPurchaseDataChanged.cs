using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductPurchaseDataChanged
{
    public ProductPurchaseDataChanged()
    {
    }

    [SetsRequiredMembers]
    public ProductPurchaseDataChanged(
        string productId,
        DateTimeOffset? dateOfPurchase,
        string? invoiceNumber, 
        decimal? purchasePrice,
        Guid dateOfPurchaseChangedBy,
        DateTimeOffset? dateOfPurchaseChangedAt = null)
    {
        ProductId = productId;
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        DateOfPurchaseChangedBy = dateOfPurchaseChangedBy;
        DateOfPurchaseChangedAt = dateOfPurchaseChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required DateTimeOffset? DateOfPurchase { get; init; }
    public required string? InvoiceNumber { get; init; }
    public required decimal? PurchasePrice { get; init; }
    public required Guid DateOfPurchaseChangedBy { get; init; }
    public required DateTimeOffset DateOfPurchaseChangedAt { get; init; }
}