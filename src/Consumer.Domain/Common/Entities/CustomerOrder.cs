using System.Runtime.Serialization;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Common.Entities;

public sealed record CustomerOrder
{
    [IgnoreDataMember]
    public CustomerId CustomerId { get; init; } = new(string.Empty);
    [IgnoreDataMember]
    public Customer? Customer { get; set; }
    public OrderId OrderId { get; init; } = null!;
    public CentreId CentreId { get; init; } = null!;
}