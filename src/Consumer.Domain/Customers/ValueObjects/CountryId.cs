using Consumer.Domain.Common.Models;

namespace Consumer.Domain.Customers.ValueObjects;

public sealed class CountryId : StronglyTypedId<string>
{
    public CountryId(string value) : base(value)
    {
    }
}