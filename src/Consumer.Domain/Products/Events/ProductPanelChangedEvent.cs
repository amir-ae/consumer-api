using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductPanelChangedEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductPanelChangedEvent(
        ProductId productId,
        string? panelModel,
        string? panelSerialNumber,
        AppUserId actor,
        DateTimeOffset? panelChangedAt = null) : base(
        productId, actor)
    {
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        PanelChangedAt = panelChangedAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string? PanelModel { get; init; }
    public required string? PanelSerialNumber { get; init; }
    public DateTimeOffset PanelChangedAt { get; init; }
}