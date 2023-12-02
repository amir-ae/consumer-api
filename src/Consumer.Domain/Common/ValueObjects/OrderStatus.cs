namespace Consumer.Domain.Common.ValueObjects;

public enum OrderStatus
{
    Pending = 1,
    Diagnosed = 8,
    Maintained = 16,
    Delivered = 32,
    Closed = 64
}