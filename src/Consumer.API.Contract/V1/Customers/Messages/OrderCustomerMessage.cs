using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Customers.Messages;

public sealed record OrderCustomerMessage
{
    [SetsRequiredMembers]
    public OrderCustomerMessage(
        string customerId,
        string customerName,
        int customerRole)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        CustomerRole = customerRole;
    }
    
    public required string CustomerId { get; init; }
    public required string CustomerName { get; init; }
    public required int CustomerRole { get; init; }
}