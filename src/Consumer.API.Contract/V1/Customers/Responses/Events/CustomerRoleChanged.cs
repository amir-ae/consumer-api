using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerRoleChanged
{
    public CustomerRoleChanged()
    {
    }

    [SetsRequiredMembers]
    public CustomerRoleChanged(
        string customerId,
        int role,
        Guid changedBy,
        DateTimeOffset? changedAt = null)
    {
        CustomerId = customerId;
        Role = role;
        ChangedBy = changedBy;
        ChangedAt = changedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required int Role { get; init; }
    public required Guid ChangedBy { get; init; }
    public required DateTimeOffset ChangedAt { get; init; }
}