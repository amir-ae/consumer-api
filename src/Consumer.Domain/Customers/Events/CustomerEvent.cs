using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Domain.Customers.Events;

public abstract record CustomerEvent
{
    protected CustomerEvent()
    {
    }

    [SetsRequiredMembers]
    protected CustomerEvent(
        CustomerId customerId)
    {
        CustomerId = customerId;
    }
    
    public required CustomerId CustomerId { get; init; }
}