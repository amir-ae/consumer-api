using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Common.Entities;

public sealed record CustomerProduct
{
    public CustomerId CustomerId { get; init; } = null!;
    public Customer? Customer { get; set; }
    public ProductId ProductId { get; init; } = null!;
    public Product? Product { get; set; }
}