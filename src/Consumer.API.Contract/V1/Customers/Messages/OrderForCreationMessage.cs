using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Messages;

public record OrderForCreationMessage
{
    public OrderForCreationMessage()
    {
    }

    [SetsRequiredMembers]
    public OrderForCreationMessage(
        Guid appUserId,
        string orderId,
        Guid centreId,
        OrderCustomerMessage customer,
        OrderProductMessage product,
        DateTimeOffset dateOfOrder,
        decimal? estimatedCost)
    {
        AppUserId = appUserId;
        OrderId = orderId;
        CentreId = centreId;
        Customer = customer;
        Product = product;
        DateOfOrder = dateOfOrder;
        EstimatedCost = estimatedCost;
    }
    
    public required Guid AppUserId { get; init; }
    public required string OrderId { get; init; }
    public required Guid CentreId { get; init; }
    public required OrderCustomerMessage Customer { get; init; }
    public required OrderProductMessage Product { get; init; }
    public required DateTimeOffset DateOfOrder { get; init; }
    public decimal? EstimatedCost { get; init; }
}