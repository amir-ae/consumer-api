using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductOrderRemoved
{
    public ProductOrderRemoved()
    {
    }

    [SetsRequiredMembers]
    public ProductOrderRemoved(
        string productId,
        ProductOrder order,
        Guid orderRemovedBy,
        DateTimeOffset? orderRemovedAt = null)
    {
        ProductId = productId;
        Order = order;
        OrderRemovedBy = orderRemovedBy;
        OrderRemovedAt = orderRemovedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required ProductOrder Order { get; init; }
    public required Guid OrderRemovedBy { get; init; }
    public required DateTimeOffset OrderRemovedAt { get; init; }
}