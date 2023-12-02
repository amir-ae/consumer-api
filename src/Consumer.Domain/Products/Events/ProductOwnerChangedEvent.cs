using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductOwnerChangedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductOwnerChangedEvent(
        ProductId productId,
        CustomerId? ownerId,
        string? ownerName,
        AppUserId actor,
        DateTimeOffset? ownerChangedAt = null) : base(
        productId, actor)
    {
        OwnerId = ownerId;
        OwnerName = ownerName;
        OwnerChangedAt = ownerChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required CustomerId? OwnerId { get; init; }
    public required string? OwnerName { get; init; }
    public DateTimeOffset OwnerChangedAt { get; init; }
}