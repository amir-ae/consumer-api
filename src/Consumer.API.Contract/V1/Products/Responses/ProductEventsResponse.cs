using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Products.Responses.Events;

namespace Consumer.API.Contract.V1.Products.Responses;

public sealed record ProductEventsResponse
{
    public ProductEventsResponse()
    {
    }

    [SetsRequiredMembers]
    public ProductEventsResponse(
        ProductCreated productCreatedEvent,
        List<ProductBrandChanged>? productBrandChangedEvents = null,
        List<ProductModelChanged>? productModelChangedEvents = null,
        List<ProductOwnerChanged>? productOwnerChangedEvents = null,
        List<ProductDealerChanged>? productDealerChangedEvents = null,
        List<ProductOrderAdded>? productOrderAddedEvents = null,
        List<ProductOrderRemoved>? productOrderRemovedEvents = null,
        List<ProductDeviceTypeChanged>? productDeviceTypeChangedEvents = null,
        List<ProductPanelChanged>? productPanelChangedEvents = null,
        List<ProductWarrantyCardNumberChanged>? productWarrantyCardNumberChangedEvents = null,
        List<ProductPurchaseDataChanged>? productDateOfPurchaseChangedEvents = null,
        List<ProductUnrepairable>? productUnrepairableEvents = null,
        List<ProductActivated>? productActivatedEvents = null,
        List<ProductDeactivated>? productDeactivatedEvents = null,
        List<ProductDeleted>? productDeletedEvents = null,
        List<ProductUndeleted>? productUndeletedEvents = null)
    {
        ProductCreatedEvent = productCreatedEvent;
        ProductBrandChangedEvents = productBrandChangedEvents ??  new();
        ProductModelChangedEvents = productModelChangedEvents ??  new();
        ProductOwnerChangedEvents = productOwnerChangedEvents ??  new();
        ProductDealerChangedEvents = productDealerChangedEvents ??  new();
        ProductOrderAddedEvents = productOrderAddedEvents ??  new();
        ProductOrderRemovedEvents = productOrderRemovedEvents ??  new();
        ProductDeviceTypeChangedEvents = productDeviceTypeChangedEvents ??  new();
        ProductPanelChangedEvents = productPanelChangedEvents ??  new();
        ProductWarrantyCardNumberChangedEvents = productWarrantyCardNumberChangedEvents ??  new();
        ProductDateOfPurchaseChangedEvents = productDateOfPurchaseChangedEvents ??  new();
        ProductUnrepairableEvents = productUnrepairableEvents ??  new();
        ProductActivatedEvents = productActivatedEvents ?? new();
        ProductDeactivatedEvents = productDeactivatedEvents ?? new();
        ProductDeletedEvents = productDeletedEvents ?? new();
        ProductUndeletedEvents = productUndeletedEvents ?? new();
    }
    
    public required ProductCreated ProductCreatedEvent { get; init; }
    public required List<ProductBrandChanged> ProductBrandChangedEvents { get; init; }
    public required List<ProductModelChanged> ProductModelChangedEvents { get; init; }
    public required List<ProductOwnerChanged> ProductOwnerChangedEvents { get; init; }
    public required List<ProductDealerChanged> ProductDealerChangedEvents { get; init; }
    public required List<ProductOrderAdded> ProductOrderAddedEvents { get; init; }
    public required List<ProductOrderRemoved> ProductOrderRemovedEvents { get; init; }
    public required List<ProductDeviceTypeChanged> ProductDeviceTypeChangedEvents { get; init; }
    public required List<ProductPanelChanged> ProductPanelChangedEvents { get; init; }
    public required List<ProductWarrantyCardNumberChanged> ProductWarrantyCardNumberChangedEvents { get; init; }
    public required List<ProductPurchaseDataChanged> ProductDateOfPurchaseChangedEvents { get; init; }
    public required List<ProductUnrepairable> ProductUnrepairableEvents { get; init; }
    public required List<ProductActivated> ProductActivatedEvents { get; init; }
    public required List<ProductDeactivated> ProductDeactivatedEvents { get; init; }
    public required List<ProductDeleted> ProductDeletedEvents { get; init; }
    public required List<ProductUndeleted> ProductUndeletedEvents { get; init; }
}