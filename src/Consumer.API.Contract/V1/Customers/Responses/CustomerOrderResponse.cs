using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Responses;

namespace Consumer.API.Contract.V1.Customers.Responses;

public sealed record CustomerOrderResponse : AuditableResponse
{
    public CustomerOrderResponse()
    {
    }

    [SetsRequiredMembers]
    public CustomerOrderResponse(
        string orderId,
        int status,
        Guid centreId,
        CustomerResponse customer,
        ProductResponse product,
        string completeness,
        string appearance,
        string malfunction,
        bool warranty,
        decimal? estimatedCost,
        DateTimeOffset createdAt,
        Guid createdBy,
        DateTimeOffset? lastModifiedAt,
        Guid? lastModifiedBy,
        bool isActive,
        bool isDeleted) : base(
        createdAt, 
        createdBy, 
        lastModifiedAt, 
        lastModifiedBy,
        isActive, 
        isDeleted)
    {
        OrderId = orderId;
        Status = status;
        CentreId = centreId;
        Customer = customer;
        Product = product;
        Completeness = completeness;
        Appearance = appearance;
        Malfunction = malfunction;
        Warranty = warranty;
        EstimatedCost = estimatedCost;
    }

    public required string OrderId { get; init; }
    public required int Status { get; init; }
    public required Guid CentreId { get; init; }
    public required CustomerResponse Customer { get; init; }
    public required ProductResponse Product { get; init; }
    public required string Completeness { get; init; }
    public required string Appearance { get; init; }
    public required string Malfunction { get; init; }
    public required bool Warranty { get; init; }
    public required decimal? EstimatedCost { get; init; }
}