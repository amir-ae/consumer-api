using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Messages;

public record ProductUpdateMessage
{
    public ProductUpdateMessage()
    {
    }

    [SetsRequiredMembers]
    public ProductUpdateMessage(
        Guid appUserId,
        string productId,
        string? brand,
        string? model,
        DateTimeOffset? updatedAt)
    {
        AppUserId = appUserId;
        ProductId = productId;
        Brand = brand;
        Model = model;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required Guid AppUserId { get; init; }
    public required string ProductId { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public required DateTimeOffset? UpdatedAt { get; init; }
}