using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Messages;

public record ProductUpdateMessage
{
    [SetsRequiredMembers]
    public ProductUpdateMessage(
        string productId,
        string? brand,
        string? model,
        Guid updatedBy,
        DateTimeOffset? updatedAt)
    {
        ProductId = productId;
        Brand = brand;
        Model = model;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public required Guid UpdatedBy { get; init; }
    public required DateTimeOffset? UpdatedAt { get; init; }
}