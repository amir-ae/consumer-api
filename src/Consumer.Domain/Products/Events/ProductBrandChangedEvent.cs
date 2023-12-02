using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductBrandChangedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductBrandChangedEvent(
        ProductId productId,
        string brand,
        AppUserId actor,
        DateTimeOffset? brandChangedAt = null) : base(
        productId, actor)
    {
        Brand = brand;
        BrandChangedAt = brandChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string Brand { get; init; }
    public DateTimeOffset BrandChangedAt { get; init; }
}