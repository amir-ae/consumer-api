using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductDeletedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductDeletedEvent(
        ProductId productId,
        AppUserId actor,
        DateTimeOffset? deletedAt = null) : base(
        productId, actor)
    {
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public DateTimeOffset DeletedAt { get; init; }
}