using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductBrandChangedEvent : ProductEvent
{
    public ProductBrandChangedEvent()
    {
    }

    [SetsRequiredMembers]
    public ProductBrandChangedEvent(
        ProductId productId,
        string brand,
        AppUserId brandChangedBy,
        DateTimeOffset? brandChangedAt = null) : base(
        productId)
    {
        Brand = brand;
        BrandChangedBy = brandChangedBy;
        BrandChangedAt = brandChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string Brand { get; init; }
    public required AppUserId BrandChangedBy { get; init; }
    public required DateTimeOffset BrandChangedAt { get; init; }
}