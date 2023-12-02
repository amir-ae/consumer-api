using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public sealed record CustomerProductRemovedEvent : CustomerEvent
{
    public CustomerProductRemovedEvent()
    {
    }

    [SetsRequiredMembers]
    public CustomerProductRemovedEvent(
        CustomerId customerId,
        ProductId productId,
        AppUserId productRemovedBy,
        DateTimeOffset? productRemovedAt = null) : base(
        customerId)
    {
        ProductId = productId;
        ProductRemovedBy = productRemovedBy;
        ProductRemovedAt = productRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required ProductId ProductId { get; init; }
    public required AppUserId ProductRemovedBy { get; init; }
    public required DateTimeOffset ProductRemovedAt { get; init; }
}