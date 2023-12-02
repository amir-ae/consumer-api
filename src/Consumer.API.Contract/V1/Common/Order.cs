using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Common;

public record Order
{
    [SetsRequiredMembers]
    public Order(
        string id,
        Guid centreId)
    {
        Id = id;
        CentreId = centreId;
    }
        
    public string Id { get; init; }
    public Guid CentreId { get; init; }
}