using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public abstract record ProductEvent
{
    [SetsRequiredMembers]
    protected ProductEvent(
        ProductId productId,
        AppUserId actor)
    {
        ProductId = productId;
        Actor = actor;
    }
    
    public required ProductId ProductId { get; init; }
    public required AppUserId Actor { get; init; }
}