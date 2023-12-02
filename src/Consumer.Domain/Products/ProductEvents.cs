using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Products.Events;

namespace Consumer.Domain.Products;

public sealed record ProductEvents
{
    public ProductEvents()
    {
    }

    [SetsRequiredMembers]
    public ProductEvents(
        ProductCreatedEvent productCreatedEvent,
        List<ProductBrandChangedEvent>? productBrandChangedEvents = null,
        List<ProductModelChangedEvent>? productModelChangedEvents = null,
        List<ProductOwnerChangedEvent>? productOwnerChangedEvents = null,
        List<ProductDealerChangedEvent>? productDealerChangedEvents = null,
        List<ProductOrderAddedEvent>? productOrderAddedEvents = null,
        List<ProductOrderRemovedEvent>? productOrderRemovedEvents = null,
        List<ProductDeviceTypeChangedEvent>? productDeviceTypeChangedEvents = null,
        List<ProductPanelChangedEvent>? productPanelChangedEvents = null,
        List<ProductWarrantyCardNumberChangedEvent>? productWarrantyCardNumberChangedEvents = null,
        List<ProductPurchaseDataChangedEvent>? productDateOfPurchaseChangedEvents = null,
        List<ProductUnrepairableEvent>? productUnrepairableEvents = null,
        List<ProductActivatedEvent>? productActivatedEvents = null,
        List<ProductDeactivatedEvent>? productDeactivatedEvents = null,
        List<ProductDeletedEvent>? productDeletedEvents = null,
        List<ProductUndeletedEvent>? productUndeletedEvents = null)
    {
        ProductCreatedEvent = productCreatedEvent;
        ProductBrandChangedEvents = productBrandChangedEvents ?? new();
        ProductModelChangedEvents = productModelChangedEvents ?? new();
        ProductOwnerChangedEvents = productOwnerChangedEvents ?? new();
        ProductDealerChangedEvents = productDealerChangedEvents ?? new();
        ProductOrderAddedEvents = productOrderAddedEvents ?? new();
        ProductOrderRemovedEvents = productOrderRemovedEvents ?? new();
        ProductDeviceTypeChangedEvents = productDeviceTypeChangedEvents ?? new();
        ProductPanelChangedEvents = productPanelChangedEvents ?? new();
        ProductWarrantyCardNumberChangedEvents = productWarrantyCardNumberChangedEvents ?? new();
        ProductDateOfPurchaseChangedEvents = productDateOfPurchaseChangedEvents ?? new();
        ProductUnrepairableEvents = productUnrepairableEvents ?? new();
        ProductActivatedEvents = productActivatedEvents ?? new();
        ProductDeactivatedEvents = productDeactivatedEvents ?? new();
        ProductDeletedEvents = productDeletedEvents ?? new();
        ProductUndeletedEvents = productUndeletedEvents ?? new();
    }
    
    public required ProductCreatedEvent ProductCreatedEvent { get; init; }
    public required List<ProductBrandChangedEvent> ProductBrandChangedEvents { get; set; }
    public required List<ProductModelChangedEvent> ProductModelChangedEvents { get; set; }
    public required List<ProductOwnerChangedEvent> ProductOwnerChangedEvents { get; set; }
    public required List<ProductDealerChangedEvent> ProductDealerChangedEvents { get; set; }
    public required List<ProductOrderAddedEvent> ProductOrderAddedEvents { get; set; }
    public required List<ProductOrderRemovedEvent> ProductOrderRemovedEvents { get; set; }
    public required List<ProductDeviceTypeChangedEvent> ProductDeviceTypeChangedEvents { get; set; }
    public required List<ProductPanelChangedEvent> ProductPanelChangedEvents { get; set; }
    public required List<ProductWarrantyCardNumberChangedEvent> ProductWarrantyCardNumberChangedEvents { get; set; }
    public required List<ProductPurchaseDataChangedEvent> ProductDateOfPurchaseChangedEvents { get; set; }
    public required List<ProductUnrepairableEvent> ProductUnrepairableEvents { get; set; }
    public required List<ProductActivatedEvent> ProductActivatedEvents { get; set; }
    public required List<ProductDeactivatedEvent> ProductDeactivatedEvents { get; set; }
    public required List<ProductDeletedEvent> ProductDeletedEvents { get; set; }
    public required List<ProductUndeletedEvent> ProductUndeletedEvents { get; set; }
    
}