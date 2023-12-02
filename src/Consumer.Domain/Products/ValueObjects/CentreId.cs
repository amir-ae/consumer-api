using Consumer.Domain.Common.Models;

namespace Consumer.Domain.Products.ValueObjects;

public sealed class CentreId : StronglyTypedId<Guid>
{
    public CentreId(Guid value) : base(value)
    {
    }
}