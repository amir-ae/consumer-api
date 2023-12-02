using System.Runtime.Serialization;
using Consumer.Domain.Common.Interfaces;
using Consumer.Domain.Common.ValueObjects;
using Marten.Metadata;

namespace Consumer.Domain.Common.Models;

public abstract record AggregateRoot<TId, T> : BaseEntity<TId>, IAuditable, IActivatable, ISoftDeletable, IRevisioned
    where TId: StronglyTypedId<T>
    where T : IComparable<T>
{
    [IgnoreDataMember]
    public T AggregateId    {
        get => Id.Value;
        protected init {}
    }

    public DateTimeOffset CreatedAt { get; protected init; } = DateTimeOffset.UtcNow;
    public AppUserId CreatedBy { get; protected init; } = new (new Guid());
    public DateTimeOffset? LastModifiedAt { get; protected init; }
    public AppUserId? LastModifiedBy { get; protected init; }
    public int Version { get; set; }
    public bool IsActive { get; protected init; } = true;
    public bool IsDeleted { get; protected init; }
}