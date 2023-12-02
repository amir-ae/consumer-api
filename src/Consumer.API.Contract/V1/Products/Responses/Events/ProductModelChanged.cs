using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductModelChanged
{
    public ProductModelChanged()
    {
    }

    [SetsRequiredMembers]
    public ProductModelChanged(
        string productId,
        string model,
        Guid modelChangedBy,
        DateTimeOffset? modelChangedAt = null)
    {
        ProductId = productId;
        Model = model;
        ModelChangedBy = modelChangedBy;
        ModelChangedAt = modelChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required string Model { get; init; }
    public required Guid ModelChangedBy { get; init; }
    public required DateTimeOffset ModelChangedAt { get; init; }
}