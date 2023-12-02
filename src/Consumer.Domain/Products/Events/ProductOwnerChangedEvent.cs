using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductOwnerChangedEvent : ProductEvent
{
    public ProductOwnerChangedEvent()
    {
    }

    [SetsRequiredMembers]
    public ProductOwnerChangedEvent(
        ProductId productId,
        CustomerId? ownerId,
        string? ownerName,
        AppUserId ownerChangedBy,
        DateTimeOffset? ownerChangedAt = null) : base(
        productId)
    {
        OwnerId = ownerId;
        OwnerName = ownerName;
        OwnerChangedBy = ownerChangedBy;
        OwnerChangedAt = ownerChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required CustomerId? OwnerId { get; init; }
    public required string? OwnerName { get; init; }
    public required AppUserId OwnerChangedBy { get; init; }
    public required DateTimeOffset OwnerChangedAt { get; init; }
}