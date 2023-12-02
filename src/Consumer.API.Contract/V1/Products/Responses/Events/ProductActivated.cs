using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductActivated
{
    public ProductActivated()
    {
    }

    [SetsRequiredMembers]
    public ProductActivated(
        string productId,
        Guid activatedBy,
        DateTimeOffset? activatedAt = null)
    {
        ProductId = productId;
        ActivatedBy = activatedBy;
        ActivatedAt = activatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required Guid ActivatedBy { get; init; }
    public required DateTimeOffset ActivatedAt { get; init; }
}