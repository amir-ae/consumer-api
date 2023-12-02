using Consumer.Domain.Common.Models;

namespace Consumer.Domain.Customers.ValueObjects;

public sealed class CityId : StronglyTypedId<int>
{
    public CityId(int value) : base(value)
    {
    }
}