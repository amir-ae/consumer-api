using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerDeleted
{
    public CustomerDeleted()
    {
    }

    [SetsRequiredMembers]
    public CustomerDeleted(
        string customerId,
        Guid deletedBy,
        DateTimeOffset? deletedAt = null)
    {
        CustomerId = customerId;
        DeletedBy = deletedBy;
        DeletedAt = deletedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required Guid DeletedBy { get; init; }
    public required DateTimeOffset DeletedAt { get; init; }
}