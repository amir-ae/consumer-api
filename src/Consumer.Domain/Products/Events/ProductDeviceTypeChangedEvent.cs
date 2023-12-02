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
        AppUserId deviceTypeChangedBy,
        DateTimeOffset? deviceTypeChangedAt = null) : base(
        productId)
    {
        DeviceType = deviceType;
        DeviceTypeChangedBy = deviceTypeChangedBy;
        DeviceTypeChangedAt = deviceTypeChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string? DeviceType { get; init; }
    public required AppUserId DeviceTypeChangedBy { get; init; }
    public DateTimeOffset DeviceTypeChangedAt { get; init; }
}