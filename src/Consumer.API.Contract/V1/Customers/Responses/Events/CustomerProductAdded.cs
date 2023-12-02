using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerProductAdded
{
    public CustomerProductAdded()
    {
    }

    [SetsRequiredMembers]
    public CustomerProductAdded(
        string customerId,
        string productId,
        Guid productAddedBy,
        DateTimeOffset? productAddedAt = null)
    {
        CustomerId = customerId;
        ProductId = productId;
        ProductAddedBy = productAddedBy;
        ProductAddedAt = productAddedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required string ProductId { get; init; }
    public required Guid ProductAddedBy { get; init; }
    public required DateTimeOffset ProductAddedAt { get; init; }
}