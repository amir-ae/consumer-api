using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Products.Responses;

namespace Consumer.API.Contract.V1.Customers.Responses;

public sealed record CustomerOrderResponse
{
    [SetsRequiredMembers]
    public CustomerOrderResponse(
        string orderId,
        Guid centreId,
        CustomerResponse customer,
        ProductResponse product)
    {
        OrderId = orderId;
        CentreId = centreId;
        Customer = customer;
        Product = product;
    }

    public required string OrderId { get; init; }
    public required Guid CentreId { get; init; }
    public required CustomerResponse Customer { get; init; }
    public required ProductResponse Product { get; init; }
}