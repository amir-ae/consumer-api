using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Common;

public record Order
{
    [SetsRequiredMembers]
    public Order(
        string orderId,
        Guid centreId)
    {
        OrderId = orderId;
        CentreId = centreId;
    }
        
    public string OrderId { get; init; }
    public Guid CentreId { get; init; }
}