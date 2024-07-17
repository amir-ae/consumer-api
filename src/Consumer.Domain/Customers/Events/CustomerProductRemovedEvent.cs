using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerProductRemovedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerProductRemovedEvent(
        CustomerId customerId,
        ProductId productId,
        HashSet<ProductId>? productIds,
        AppUserId actor,
        DateTimeOffset? productRemovedAt = null) : base(
        customerId, actor)
    {
        ProductId = productId;
        ProductIds = productIds ?? new();
        ProductRemovedAt = productRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required ProductId ProductId { get; init; }
    public required HashSet<ProductId> ProductIds { get; init; }
    public string ProductIdsString => string.Join(',', ProductIds.Select(p => p.Value));
    public DateTimeOffset ProductRemovedAt { get; init; }
}