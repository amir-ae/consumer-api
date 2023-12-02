using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductDeviceTypeChanged
{
    public ProductDeviceTypeChanged()
    {
    }

    [SetsRequiredMembers]
    public ProductDeviceTypeChanged(
        string productId,
        string? deviceType,
        Guid deviceTypeChangedBy,
        DateTimeOffset? deviceTypeChangedAt = null)
    {
        ProductId = productId;
        DeviceType = deviceType;
        DeviceTypeChangedBy = deviceTypeChangedBy;
        DeviceTypeChangedAt = deviceTypeChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required string? DeviceType { get; init; }
    public required Guid DeviceTypeChangedBy { get; init; }
    public required DateTimeOffset DeviceTypeChangedAt { get; init; }
}