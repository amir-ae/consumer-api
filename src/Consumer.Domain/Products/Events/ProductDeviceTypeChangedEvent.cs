using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductDeviceTypeChangedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductDeviceTypeChangedEvent(
        ProductId productId,
        string? deviceType,
        AppUserId actor,
        DateTimeOffset? deviceTypeChangedAt = null) : base(
        productId, actor)
    {
        DeviceType = deviceType;
        DeviceTypeChangedAt = deviceTypeChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string? DeviceType { get; init; }
    public DateTimeOffset DeviceTypeChangedAt { get; init; }
}