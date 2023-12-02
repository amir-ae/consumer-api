using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductDeleted
{
    public ProductDeleted()
    {
    }

    [SetsRequiredMembers]
    public ProductDeleted(
        string productId,
        Guid deletedBy,
        DateTimeOffset? deletedAt = null)
    {
        ProductId = productId;
        DeletedBy = deletedBy;
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required Guid DeletedBy { get; init; }
    public required DateTimeOffset DeletedAt { get; init; }
}