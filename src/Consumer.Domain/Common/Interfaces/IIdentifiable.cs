namespace Consumer.Domain.Common.Interfaces;

public interface IIdentifiable<T>
{
    public T Id { get; }
}