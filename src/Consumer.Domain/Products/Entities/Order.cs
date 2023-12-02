using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Entities;

public sealed record Order
{
    public OrderId OrderId { get; set; }
    public CentreId CentreId { get; set; }

    public Order(
        OrderId orderId,
        CentreId centreId)
    {
        OrderId = orderId;
        CentreId = centreId;
    }
    
    public static Order Create(
        OrderId orderId,
        CentreId centreId)
    {
        return new Order(
            orderId,
            centreId);
    }
    
#pragma warning disable CS8618
    private Order() { }
#pragma warning restore CS8618
}