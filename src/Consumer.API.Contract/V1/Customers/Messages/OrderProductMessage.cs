using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Messages;

public sealed record OrderProductMessage
{
    [SetsRequiredMembers]
    public OrderProductMessage(
        string productId,
        string brand,
        string model,
        bool warranty,
        string completeness,
        string appearance,
        string malfunction)
    {
        ProductId = productId;
        Brand = brand;
        Model = model;
        Completeness = completeness;
        Appearance = appearance;
        Malfunction = malfunction;
        Warranty = warranty;
    }
    
    public required string ProductId { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public required string Completeness { get; init; }
    public required string Appearance { get; init; }
    public required string Malfunction { get; init; }
    public required bool Warranty { get; init; }
}