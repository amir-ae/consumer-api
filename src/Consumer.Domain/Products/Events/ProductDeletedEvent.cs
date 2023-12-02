using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductDeletedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductDeletedEvent(
        ProductId productId,
        AppUserId deletedBy,
        DateTimeOffset? deletedAt = null) : base(
        productId)
    {
        DeletedBy = deletedBy;
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required AppUserId DeletedBy { get; init; }
    public DateTimeOffset DeletedAt { get; init; }
}