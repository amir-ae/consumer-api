using Consumer.Domain.Common.Models;

namespace Consumer.Domain.Products.ValueObjects;

public sealed class OrderId : StronglyTypedId<string>
{
    public OrderId(string value) : base(value)
    {
    }
}