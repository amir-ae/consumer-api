using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductOwnerChanged
{
    public ProductOwnerChanged()
    {
    }

    [SetsRequiredMembers]
    public ProductOwnerChanged(
        string productId,
        string? ownerId,
        string? ownerName,
        Guid ownerChangedBy,
        DateTimeOffset? ownerChangedAt = null)
    {
        ProductId = productId;
        OwnerId = ownerId;
        OwnerName = ownerName;
        OwnerChangedBy = ownerChangedBy;
        OwnerChangedAt = ownerChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required string? OwnerId { get; init; }
    public required string? OwnerName { get; init; }
    public required Guid OwnerChangedBy { get; init; }
    public required DateTimeOffset OwnerChangedAt { get; init; }
}