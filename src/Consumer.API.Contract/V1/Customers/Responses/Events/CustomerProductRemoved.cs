using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerProductRemoved
{
    public CustomerProductRemoved()
    {
    }

    [SetsRequiredMembers]
    public CustomerProductRemoved(
        string customerId,
        string productId,
        Guid productRemovedBy,
        DateTimeOffset? productRemovedAt = null)
    {
        CustomerId = customerId;
        ProductId = productId;
        ProductRemovedBy = productRemovedBy;
        ProductRemovedAt = productRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required string ProductId { get; init; }
    public required Guid ProductRemovedBy { get; init; }
    public required DateTimeOffset ProductRemovedAt { get; init; }
}