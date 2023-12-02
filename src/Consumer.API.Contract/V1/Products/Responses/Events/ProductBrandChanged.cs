using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductBrandChanged
{
    public ProductBrandChanged()
    {
    }

    [SetsRequiredMembers]
    public ProductBrandChanged(
        string productId,
        string brand,
        Guid brandChangedBy,
        DateTimeOffset? brandChangedAt = null)
    {
        ProductId = productId;
        Brand = brand;
        BrandChangedBy = brandChangedBy;
        BrandChangedAt = brandChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required string Brand { get; init; }
    public required Guid BrandChangedBy { get; init; }
    public required DateTimeOffset BrandChangedAt { get; init; }
}