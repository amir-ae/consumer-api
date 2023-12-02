using Consumer.Domain.Common.Models;

namespace Consumer.Domain.Products.ValueObjects;

public sealed class ProductId : StronglyTypedId<string>
{
    public ProductId(string value) : base(value)
    {
    }
}