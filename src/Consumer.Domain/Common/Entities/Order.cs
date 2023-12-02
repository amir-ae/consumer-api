using Consumer.Domain.Common.Models;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Common.Entities;

public sealed record Order : BaseEntity<OrderId>
{
    public CentreId CentreId { get; private set; }

    public Order(
        OrderId id,
        CentreId centreId)
    {
        Id = id;
        CentreId = centreId;
    }
    
    public static Order Create(
        OrderId id,
        CentreId centreId)
    {
        return new Order(
            id,
            centreId);
    }
    
#pragma warning disable CS8618
    private Order() { }
#pragma warning restore CS8618
}