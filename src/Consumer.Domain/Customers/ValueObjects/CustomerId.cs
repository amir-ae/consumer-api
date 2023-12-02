using Consumer.Domain.Common.Models;

namespace Consumer.Domain.Customers.ValueObjects;

public sealed class CustomerId : StronglyTypedId<string>
{
    public CustomerId(string value) : base(value)
    {
    }
}