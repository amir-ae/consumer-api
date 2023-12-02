using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Messages;

public record CustomerUpdateMessage
{
    [SetsRequiredMembers]
    public CustomerUpdateMessage(
        string customerId,
        string customerName,
        Guid updatedBy,
        DateTimeOffset? updatedAt)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string CustomerId { get; init; }
    public required string CustomerName { get; init; }
    public required Guid UpdatedBy { get; init; }
    public required DateTimeOffset? UpdatedAt { get; init; }
}