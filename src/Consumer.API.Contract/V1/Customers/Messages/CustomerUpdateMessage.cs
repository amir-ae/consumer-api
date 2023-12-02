using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Messages;

public record CustomerUpdateMessage
{
    public CustomerUpdateMessage()
    {
    }

    [SetsRequiredMembers]
    public CustomerUpdateMessage(
        Guid appUserId,
        string customerId,
        string customerName,
        DateTimeOffset? updatedAt)
    {
        AppUserId = appUserId;
        CustomerId = customerId;
        CustomerName = customerName;
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required Guid AppUserId { get; init; }
    public required string CustomerId { get; init; }
    public required string CustomerName { get; init; }
    public required DateTimeOffset? UpdatedAt { get; init; }
}