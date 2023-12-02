using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public abstract record CustomerEvent
{
    [SetsRequiredMembers]
    protected CustomerEvent(
        CustomerId customerId,
        AppUserId actor)
    {
        CustomerId = customerId;
        Actor = actor;
    }
    
    public required CustomerId CustomerId { get; init; }
    public required AppUserId Actor { get; init; }
}