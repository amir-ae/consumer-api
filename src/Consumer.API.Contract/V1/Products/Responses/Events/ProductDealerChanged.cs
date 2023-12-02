using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductDealerChanged
{
    public ProductDealerChanged()
    {
    }

    [SetsRequiredMembers]
    public ProductDealerChanged(
        string productId,
        string? dealerId,
        string? dealerName,
        Guid dealerChangedBy,
        DateTimeOffset? dealerChangedAt = null)
    {
        ProductId = productId;
        DealerId = dealerId;
        DealerName = dealerName;
        DealerChangedBy = dealerChangedBy;
        DealerChangedAt = dealerChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required string? DealerId { get; init; }
    public required string? DealerName { get; init; }
    public required Guid DealerChangedBy { get; init; }
    public required DateTimeOffset DealerChangedAt { get; init; }
}