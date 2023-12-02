using Consumer.Domain.Common.Models;

namespace Consumer.Domain.Common.ValueObjects;

public sealed class AppUserId : StronglyTypedId<Guid>
{
    public AppUserId(Guid value) : base(value)
    {
    }
}