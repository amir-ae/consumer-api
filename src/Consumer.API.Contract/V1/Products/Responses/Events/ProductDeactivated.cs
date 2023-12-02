using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductDeactivated
{
    public ProductDeactivated()
    {
    }

    [SetsRequiredMembers]
    public ProductDeactivated(
        string productId,
        Guid deactivatedBy,
        DateTimeOffset? deactivatedAt = null)
    {
        ProductId = productId;
        DeactivatedBy = deactivatedBy;
        DeactivatedAt = deactivatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required Guid DeactivatedBy { get; init; }
    public required DateTimeOffset DeactivatedAt { get; init; }
}