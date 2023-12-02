using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerProductAddedEvent : CustomerEvent
{
    [SetsRequiredMembers]
    public CustomerProductAddedEvent(
        CustomerId customerId,
        ProductId productId,
        AppUserId actor,
        DateTimeOffset? productAddedAt = null) : base(
        customerId, actor)
    {
        ProductId = productId;
        ProductAddedAt = productAddedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required ProductId ProductId { get; init; }
    public DateTimeOffset ProductAddedAt { get; init; }
}