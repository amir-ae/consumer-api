using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerActivated
{
    public CustomerActivated()
    {
    }

    [SetsRequiredMembers]
    public CustomerActivated(
        string customerId,
        Guid activatedBy,
        DateTimeOffset? activatedAt = null)
    {
        CustomerId = customerId;
        ActivatedBy = activatedBy;
        ActivatedAt = activatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required Guid ActivatedBy { get; init; }
    public required DateTimeOffset ActivatedAt { get; init; }
}