using System.Runtime.Serialization;
using Consumer.Domain.Common.Interfaces;
using Consumer.Domain.Common.ValueObjects;
using Marten.Metadata;

namespace Consumer.Domain.Common.Models;

public abstract class AggregateRoot<TId, T> : IAuditable, IActivatable, ISoftDeletable, IRevisioned
    where TId: StronglyTypedId<T>
    where T : IComparable<T>
{
    public TId Id { get; protected init; } = default!;
    
    [IgnoreDataMember]
    public T AggregateId    {
        get => Id.Value;
        protected set {}
    }

    public DateTimeOffset CreatedAt { get; protected set; } = DateTimeOffset.UtcNow;
    public AppUserId CreatedBy { get; protected set; } = new (new Guid());
    public DateTimeOffset? LastModifiedAt { get; protected set; }
    public AppUserId? LastModifiedBy { get; protected set; }
    public int Version { get; set; }
    public bool IsActive { get; protected set; } = true;
    public bool IsDeleted { get; protected set; }
}