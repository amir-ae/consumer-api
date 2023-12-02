namespace Consumer.Domain.Common.Models;

public abstract record BaseEntity<TId>
{
    public TId Id { get; protected init; } = default!;
}