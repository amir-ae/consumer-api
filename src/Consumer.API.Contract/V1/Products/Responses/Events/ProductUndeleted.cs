using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductUndeleted
{
    public ProductUndeleted()
    {
    }

    [SetsRequiredMembers]
    public ProductUndeleted(
        string productId,
        Guid undeletedBy,
        DateTimeOffset? undeletedAt = null)
    {
        ProductId = productId;
        UndeletedBy = undeletedBy;
        UndeletedAt = undeletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required Guid UndeletedBy { get; init; }
    public required DateTimeOffset UndeletedAt { get; init; }
}