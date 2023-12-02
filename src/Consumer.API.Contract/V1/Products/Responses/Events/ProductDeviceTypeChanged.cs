namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductDeviceTypeChanged(string ProductId,
    string? DeviceType,
    Guid DeviceTypeChangedBy,
    DateTimeOffset DeviceTypeChangedAt);