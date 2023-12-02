using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerDeactivated
{
    public CustomerDeactivated()
    {
    }

    [SetsRequiredMembers]
    public CustomerDeactivated(
        string customerId,
        Guid deactivatedBy,
        DateTimeOffset? deactivatedAt = null)
    {
        CustomerId = customerId;
        DeactivatedBy = deactivatedBy;
        DeactivatedAt = deactivatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required Guid DeactivatedBy { get; init; }
    public required DateTimeOffset DeactivatedAt { get; init; }
}