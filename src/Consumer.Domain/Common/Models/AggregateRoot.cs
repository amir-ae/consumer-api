using System.Runtime.Serialization;
using Consumer.Domain.Common.Interfaces;
using Consumer.Domain.Common.ValueObjects;

namespace Consumer.Domain.Common.Models;

public abstract record AggregateRoot<TKey, T> : IAuditable, IActivatable, ISoftDeletable, IIdentifiable<TKey>
    where TKey: StronglyTypedId<T>
    where T : IComparable<T>
{
    public TKey Id { get; set; } = default!;
    
    [IgnoreDataMember]
    public T AggregateId    {
        get => Id.Value;
        set {}
    }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public AppUserId CreatedBy { get; set; } = new (new Guid());
    public DateTimeOffset? LastModifiedAt { get; set; }
    public AppUserId? LastModifiedBy { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }
}