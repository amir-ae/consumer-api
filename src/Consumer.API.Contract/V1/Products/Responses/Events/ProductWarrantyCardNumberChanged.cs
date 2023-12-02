using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductWarrantyCardNumberChanged
{
    public ProductWarrantyCardNumberChanged()
    {
    }

    [SetsRequiredMembers]
    public ProductWarrantyCardNumberChanged(
        string productId,
        string? warrantyCardNumber,
        Guid warrantyCardNumberChangedBy,
        DateTimeOffset? warrantyCardNumberChangedAt = null)
    {
        ProductId = productId;
        WarrantyCardNumber = warrantyCardNumber;
        WarrantyCardNumberChangedBy = warrantyCardNumberChangedBy;
        WarrantyCardNumberChangedAt = warrantyCardNumberChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required string? WarrantyCardNumber { get; init; }
    public required Guid WarrantyCardNumberChangedBy { get; init; }
    public required DateTimeOffset WarrantyCardNumberChangedAt { get; init; }
}