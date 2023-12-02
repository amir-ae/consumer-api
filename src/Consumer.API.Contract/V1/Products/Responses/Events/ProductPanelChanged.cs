using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductPanelChanged
{
    public ProductPanelChanged()
    {
    }

    [SetsRequiredMembers]
    public ProductPanelChanged(
        string productId,
        string? panelModel,
        string? panelSerialNumber,
        Guid panelChangedBy,
        DateTimeOffset? panelChangedAt = null)
    {
        ProductId = productId;
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        PanelChangedBy = panelChangedBy;
        PanelChangedAt = panelChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required string? PanelModel { get; init; }
    public required string? PanelSerialNumber { get; init; }
    public required Guid PanelChangedBy { get; init; }
    public required DateTimeOffset PanelChangedAt { get; init; }
}