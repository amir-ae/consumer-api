using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public abstract record ProductEvent
{
    [SetsRequiredMembers]
    protected ProductEvent(
        ProductId productId)
    {
        ProductId = productId;
    }
    
    public required ProductId ProductId { get; init; }
}