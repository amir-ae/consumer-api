using Consumer.Domain.Common.Models;

namespace Consumer.Domain.Products.ValueObjects;

public sealed class SerialId : StronglyTypedId<int>
{
    public SerialId(int value) : base(value)
    {
    }
}