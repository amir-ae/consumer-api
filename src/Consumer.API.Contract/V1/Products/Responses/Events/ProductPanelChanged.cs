namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductPanelChanged(string ProductId,
    string? PanelModel,
    string? PanelSerialNumber,
    Guid PanelChangedBy,
    DateTimeOffset PanelChangedAt);