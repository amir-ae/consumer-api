using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Common.Entities;

public sealed record ProductOrder
{
    public ProductId ProductId { get; init; } = new(string.Empty);
    public Product? Product { get; set; }
    public OrderId OrderId { get; init; } = null!;
    public CentreId CentreId { get; init; } = null!;
}