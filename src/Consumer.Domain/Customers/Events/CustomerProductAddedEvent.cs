using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerProductAddedEvent : CustomerEvent
{
    public CustomerProductAddedEvent()
    {
    }

    [SetsRequiredMembers]
    public CustomerProductAddedEvent(
        CustomerId customerId,
        ProductId productId,
        AppUserId productAddedBy,
        DateTimeOffset? productAddedAt = null) : base(
        customerId)
    {
        ProductId = productId;
        ProductAddedBy = productAddedBy;
        ProductAddedAt = productAddedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required ProductId ProductId { get; init; }
    public required AppUserId ProductAddedBy { get; init; }
    public required DateTimeOffset ProductAddedAt { get; init; }
}