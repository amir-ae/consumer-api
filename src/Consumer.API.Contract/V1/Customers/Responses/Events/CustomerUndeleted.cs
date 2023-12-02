using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerUndeleted
{
    public CustomerUndeleted()
    {
    }

    [SetsRequiredMembers]
    public CustomerUndeleted(
        string customerId,
        Guid undeletedBy,
        DateTimeOffset? undeletedAt = null)
    {
        CustomerId = customerId;
        UndeletedBy = undeletedBy;
        UndeletedAt = undeletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required Guid UndeletedBy { get; init; }
    public required DateTimeOffset UndeletedAt { get; init; }
}