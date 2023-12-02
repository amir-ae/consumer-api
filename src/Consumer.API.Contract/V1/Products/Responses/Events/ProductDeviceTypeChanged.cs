namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductDeviceTypeChanged(string ProductId,
    string? DeviceType,
    Guid DeviceTypeChangedBy,
    DateTimeOffset DeviceTypeChangedAt);