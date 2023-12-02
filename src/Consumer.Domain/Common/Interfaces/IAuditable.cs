using Consumer.Domain.Common.ValueObjects;

namespace Consumer.Domain.Common.Interfaces;

public interface IAuditable
{
    public DateTimeOffset CreatedAt { get; }
    public AppUserId CreatedBy { get; }
    public DateTimeOffset? LastModifiedAt { get; }
    public AppUserId? LastModifiedBy { get; }
}