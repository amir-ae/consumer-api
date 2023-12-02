using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductOrderAdded
{
    public ProductOrderAdded()
    {
    }

    [SetsRequiredMembers]
    public ProductOrderAdded(
        string productId,
        ProductOrder order,
        Guid orderAddedBy,
        DateTimeOffset? orderAddedAt = null)
    {
        ProductId = productId;
        Order = order;
        OrderAddedBy = orderAddedBy;
        OrderAddedAt = orderAddedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required ProductOrder Order { get; init; }
    public required Guid OrderAddedBy { get; init; }
    public required DateTimeOffset OrderAddedAt { get; init; }
}