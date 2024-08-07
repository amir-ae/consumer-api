﻿using System.Runtime.Serialization;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Common.Models;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products;

public sealed class Product : AggregateRoot<ProductId, string>
{
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public SerialId? SerialId { get; private set; }
    public CustomerId? OwnerId { get; private set; }
    public CustomerId? DealerId { get; private set; }
    public string? DeviceType { get; private set; }
    public string? PanelModel { get; private set; }
    public string? PanelSerialNumber { get; private set; }
    public string? WarrantyCardNumber { get; private set; }
    public DateTimeOffset? DateOfPurchase { get; private set; }
    public string? InvoiceNumber { get; private set; }
    public decimal? PurchasePrice { get; private set; }
    public bool IsUnrepairable { get; private set; }
    public DateTimeOffset? DateOfDemandForCompensation { get; private set; }
    public string? DemanderFullName { get; private set; }
    public HashSet<ProductOrder> ProductOrders { get; private set; }
    [IgnoreDataMember]
    public HashSet<CustomerProduct> ProductCustomers { get; private set; } = new();
    
    public ProductEventHandler<ProductEvent> ProductEventHandler { get; } = new();

    private Product(
        ProductId id,
        string brand,
        string model,
        SerialId? serialId,
        CustomerId? ownerId,
        CustomerId? dealerId,
        string? deviceType,
        string? panelModel,
        string? panelSerialNumber,
        string? warrantyCardNumber,
        DateTimeOffset? dateOfPurchase,
        string? invoiceNumber,
        decimal? purchasePrice,
        HashSet<ProductOrder> productOrders,
        bool isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        AppUserId createdBy,
        DateTimeOffset createdAt)
    {
        Id = id;
        Brand = brand;
        Model = model;
        SerialId = serialId;
        OwnerId = ownerId;
        DealerId = dealerId;
        DeviceType = deviceType;
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        WarrantyCardNumber = warrantyCardNumber;
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        ProductOrders = productOrders;
        IsUnrepairable = isUnrepairable;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        IsActive = true;
        IsDeleted = false;
    }

    public static Product Create(ProductCreatedEvent created) =>
        new (created.ProductId,
            created.Brand,
            created.Model,
            created.SerialId,
            created.OwnerId,
            created.DealerId,
            created.DeviceType,
            created.PanelModel,
            created.PanelSerialNumber,
            created.WarrantyCardNumber,
            created.DateOfPurchase,
            created.InvoiceNumber,
            created.PurchasePrice,
            created.Orders,
            created.IsUnrepairable,
            created.DateOfDemandForCompensation,
            created.DemanderFullName,
            created.Actor,
            created.CreatedAt);

    public static async Task<Product> CreateAsync(
        ProductId id,
        string brand,
        string model,
        SerialId? serialId,
        CustomerId? ownerId,
        string? ownerName,
        CustomerId? dealerId,
        string? dealerName,
        string? deviceType,
        string? panelModel,
        string? panelSerialNumber,
        string? warrantyCardNumber,
        DateTimeOffset? dateOfPurchase,
        string? invoiceNumber,
        decimal? purchasePrice,
        HashSet<ProductOrder>? orders,
        bool? isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        AppUserId createdBy,
        DateTimeOffset? createdAt,
        Func<ProductCreatedEvent, Product>? create,
        Func<ProductCreatedEvent, CancellationToken, Task<Product>> createAsync,
        bool saveChanges = true,
        CancellationToken ct = default)
    {
        var @event = new ProductCreatedEvent(
            id,
            brand,
            model,
            serialId,
            ownerId,
            ownerName,
            dealerId,
            dealerName,
            deviceType,
            panelModel,
            panelSerialNumber,
            warrantyCardNumber,
            dateOfPurchase,
            invoiceNumber,
            purchasePrice,
            orders,
            isUnrepairable,
            dateOfDemandForCompensation,
            demanderFullName,
            createdBy,
            createdAt);

        if (!saveChanges && create is not null)
        {
            return create(@event);
        }
        
        return await createAsync(@event, ct);
    }

    public void UpdateBrand(string? brand, AppUserId updateBy, DateTimeOffset? updateAt, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrWhiteSpace(brand) && brand != Brand;
        if (!shouldUpdate) return;
        
        var @event = new ProductBrandChangedEvent(
            Id,
            brand!,
            updateBy,
            updateAt);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void UpdateModel(string? model, AppUserId updateBy, DateTimeOffset? updateAt, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrWhiteSpace(model) && model != Model;
        if (!shouldUpdate) return;
        
        var @event = new ProductModelChangedEvent(
            Id,
            model!,
            updateBy,
            updateAt);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }

    public void UpdateDeviceType(string? deviceType, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = !string.IsNullOrWhiteSpace(deviceType) && deviceType != DeviceType;
        if (!shouldUpdate) return;
        
        var @event = new ProductDeviceTypeChangedEvent(
            Id,
            deviceType,
            updateBy,
            updateAt);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void UpdatePanel(string? panelModel, string? panelSerialNumber, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = !string.IsNullOrWhiteSpace(panelModel) && panelModel != PanelModel
                           || !string.IsNullOrWhiteSpace(panelSerialNumber) && panelSerialNumber != PanelSerialNumber;
        
        if (!shouldUpdate) return;
        
        var @event = new ProductPanelChangedEvent(
            Id,
            panelModel ?? PanelModel,
            panelSerialNumber ?? PanelSerialNumber,
            updateBy,
            updateAt);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void UpdateWarrantyCardNumber(string? warrantyCardNumber, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = !string.IsNullOrWhiteSpace(warrantyCardNumber) && warrantyCardNumber != WarrantyCardNumber;
        if (!shouldUpdate) return;
        
        var @event = new ProductWarrantyCardNumberChangedEvent(
            Id,
            warrantyCardNumber,
            updateBy,
            updateAt);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void UpdatePurchaseData(DateTimeOffset? dateOfPurchase, string? invoiceNumber, decimal? purchasePrice, 
        AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = dateOfPurchase is not null && dateOfPurchase != DateOfPurchase
                       || !string.IsNullOrWhiteSpace(invoiceNumber) && invoiceNumber != InvoiceNumber
                       || purchasePrice is not null && purchasePrice != PurchasePrice;
        if (!shouldUpdate) return;
        
        var @event = new ProductPurchaseDataChangedEvent(
            Id,
            dateOfPurchase ?? DateOfPurchase,
            invoiceNumber ?? InvoiceNumber,
            purchasePrice ?? PurchasePrice,
            updateBy,
            updateAt);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void UpdateUnrepairable(bool? isUnrepairable, DateTimeOffset? dateOfDemandForCompensation, 
        string? demanderFullName, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = isUnrepairable is not null && isUnrepairable != IsUnrepairable
                       || dateOfDemandForCompensation is not null && dateOfDemandForCompensation != DateOfDemandForCompensation
                       || !string.IsNullOrWhiteSpace(demanderFullName) && demanderFullName != DemanderFullName;
        if (!shouldUpdate) return;
        
        var @event = new ProductUnrepairableEvent(
            Id,
            isUnrepairable ?? IsUnrepairable,
            dateOfDemandForCompensation ?? DateOfDemandForCompensation,
            demanderFullName ?? DemanderFullName,
            updateBy,
            updateAt);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void AddOrders(HashSet<ProductOrder>? orders, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = orders is not null && !orders.SetEquals(ProductOrders);
        if (!shouldUpdate) return;
        
        var ordersToAdd = orders!.Except(ProductOrders);
        foreach (var order in ordersToAdd)
        {
            var updatedOrders = new HashSet<ProductOrder>(ProductOrders) { order };
            
            var @event = new ProductOrderAddedEvent(
                Id,
                order,
                updatedOrders,
                updateBy,
                updateAt);

            Apply(@event);
            ProductEventHandler.RaiseEvent(@event);
        }
    }
    
    public void RemoveOrders(HashSet<ProductOrder>? orders, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = orders is not null && !orders.SetEquals(ProductOrders);
        if (!shouldUpdate) return;
        
        var ordersToRemove = ProductOrders.Except(orders!);
        foreach (var order in ordersToRemove)
        {
            var updatedOrders = new HashSet<ProductOrder>(ProductOrders);
            updatedOrders.Remove(order);
            
            var @event = new ProductOrderRemovedEvent(
                Id,
                order,
                updatedOrders,
                updateBy,
                updateAt);
            
            Apply(@event);
            ProductEventHandler.RaiseEvent(@event);
        }
    }
    
    public void UpdateOwner(CustomerId? ownerId, string? ownerName, AppUserId updateBy, DateTimeOffset? updateAt, 
        out bool shouldUpdate)
    {
        shouldUpdate = ownerId != OwnerId;
        if (!shouldUpdate) return;
        
        var @event = new ProductOwnerChangedEvent(
            Id,
            ownerId?.Value == string.Empty ? null : ownerId,
            ownerName,
            updateBy,
            updateAt);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void UpdateDealer(CustomerId? dealerId, string? ownerName, AppUserId updateBy, DateTimeOffset? updateAt, 
        out bool shouldUpdate)
    {
        shouldUpdate = dealerId != DealerId;
        if (!shouldUpdate) return;
        
        var @event = new ProductDealerChangedEvent(
            Id,
            dealerId?.Value == string.Empty ? null : dealerId,
            ownerName,
            updateBy,
            updateAt);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void Activate(AppUserId activateBy, Action<ProductActivatedEvent> append)
    {
        if (IsActive) return;
        
        var @event = new ProductActivatedEvent(Id, activateBy);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void Deactivate(AppUserId deactivateBy, Action<ProductDeactivatedEvent> append)
    {
        if (!IsActive) return;
        
        var @event = new ProductDeactivatedEvent(Id, deactivateBy);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void Delete(AppUserId deleteBy)
    {
        if (IsDeleted) return;
        
        var @event = new ProductDeletedEvent(Id, deleteBy);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }
    
    public void Undelete(AppUserId undeleteBy)
    {
        if (!IsDeleted) return;
        
        var @event = new ProductUndeletedEvent(Id, undeleteBy);
        
        Apply(@event);
        ProductEventHandler.RaiseEvent(@event);
    }

    public void AddOwner(Customer? owner)
    {
        if (owner is null) return;
        ProductCustomers.Add(new CustomerProduct { CustomerId = owner.Id, Customer = owner, ProductId = Id });
    }
    
    public void AddDealer(Customer? dealer)
    {
        if (dealer is null) return;
        ProductCustomers.Add(new CustomerProduct { CustomerId = dealer.Id, Customer = dealer, ProductId = Id });
    }

    public void Apply(ProductBrandChangedEvent changed)
    {
        Brand = changed.Brand;
        SerialId = null;
        LastModifiedAt = changed.BrandChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(ProductModelChangedEvent changed)
    {
        Model = changed.Model;
        SerialId = null;
        LastModifiedAt = changed.ModelChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(ProductOwnerChangedEvent changed)
    {
        OwnerId = changed.OwnerId;
        LastModifiedAt = changed.OwnerChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(ProductDealerChangedEvent changed)
    {
        DealerId = changed.DealerId;
        LastModifiedAt = changed.DealerChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(ProductOrderAddedEvent added)
    {
        ProductOrders.Add(added.Order);
        LastModifiedAt = added.OrderAddedAt;
        LastModifiedBy = added.Actor;
        Version++;
    }

    public void Apply(ProductOrderRemovedEvent removed)
    {
        ProductOrders.Remove(removed.Order);
        LastModifiedAt = removed.OrderRemovedAt;
        LastModifiedBy = removed.Actor;
        Version++;
    }

    public void Apply(ProductDeviceTypeChangedEvent changed)
    {
        DeviceType = changed.DeviceType;
        LastModifiedAt = changed.DeviceTypeChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(ProductPanelChangedEvent changed)
    {
        PanelModel = changed.PanelModel;
        PanelSerialNumber = changed.PanelSerialNumber;
        LastModifiedAt = changed.PanelChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(ProductWarrantyCardNumberChangedEvent changed)
    {
        WarrantyCardNumber = changed.WarrantyCardNumber;
        LastModifiedAt = changed.WarrantyCardNumberChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }


    public void Apply(ProductPurchaseDataChangedEvent changed)
    {
        DateOfPurchase = changed.DateOfPurchase;
        InvoiceNumber = changed.InvoiceNumber;
        PurchasePrice = changed.PurchasePrice;
        LastModifiedAt = changed.PurchaseDataChangedAt;
        LastModifiedBy = changed.Actor;
        Version++;
    }

    public void Apply(ProductUnrepairableEvent unrepairable)
    {
        IsUnrepairable = unrepairable.IsUnrepairable;
        DateOfDemandForCompensation = unrepairable.DateOfDemandForCompensation;
        DemanderFullName = unrepairable.DemanderFullName;
        LastModifiedAt = unrepairable.UnrepairableAt;
        LastModifiedBy = unrepairable.Actor;
        Version++;
    }

    public void Apply(ProductActivatedEvent activated)
    {
        IsActive = true;
        LastModifiedAt = activated.ActivatedAt;
        LastModifiedBy = activated.Actor;
        Version++;
    }

    public void Apply(ProductDeactivatedEvent deactivated)
    {
        IsActive = false;
        LastModifiedAt = deactivated.DeactivatedAt;
        LastModifiedBy = deactivated.Actor;
        Version++;
    }

    public void Apply(ProductDeletedEvent deleted)
    {
        IsActive = false;
        IsDeleted = true;
        LastModifiedAt = deleted.DeletedAt;
        LastModifiedBy = deleted.Actor;
        Version++;
    }

    public void Apply(ProductUndeletedEvent undeleted)
    {
        IsActive = true;
        IsDeleted = false;
        LastModifiedAt = undeleted.UndeletedAt;
        LastModifiedBy = undeleted.Actor;
        Version++;
    }

    public void Apply(ProductEvent @event)
    {
        switch (@event)
        {
            case ProductBrandChangedEvent brandChangedEvent:
                Apply(brandChangedEvent);
                break;
            case ProductModelChangedEvent modelChangedEvent:
                Apply(modelChangedEvent);
                break;
            case ProductOwnerChangedEvent ownerChangedEvent:
                Apply(ownerChangedEvent);
                break;
            case ProductDealerChangedEvent dealerChangedEvent:
                Apply(dealerChangedEvent);
                break;
            case ProductOrderAddedEvent orderAddedEvent:
                Apply(orderAddedEvent);
                break;
            case ProductDeviceTypeChangedEvent deviceTypeChangedEvent:
                Apply(deviceTypeChangedEvent);
                break;
            case ProductPanelChangedEvent panelChangedEvent:
                Apply(panelChangedEvent);
                break;
            case ProductWarrantyCardNumberChangedEvent warrantyCardNumberChangedEvent:
                Apply(warrantyCardNumberChangedEvent);
                break;
            case ProductPurchaseDataChangedEvent purchaseDataChangedEvent:
                Apply(purchaseDataChangedEvent);
                break;
            case ProductUnrepairableEvent unrepairableEvent:
                Apply(unrepairableEvent);
                break;
            case ProductActivatedEvent activatedEvent:
                Apply(activatedEvent);
                break;
            case ProductDeactivatedEvent deactivatedEvent:
                Apply(deactivatedEvent);
                break;
            case ProductDeletedEvent deletedEvent:
                Apply(deletedEvent);
                break;
            case ProductUndeletedEvent undeletedEvent:
                Apply(undeletedEvent);
                break;
            default:
                throw new InvalidOperationException($"Unhandled product event type {@event.GetType()}");
        }
    }

#pragma warning disable CS8618
    private Product() { }
#pragma warning restore CS8618
}