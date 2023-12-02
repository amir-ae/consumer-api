using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Common.Models;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products;

public sealed record Product : AggregateRoot<ProductId, string>
{
    public string Brand { get; private init; }
    public string Model { get; private init; }
    public SerialId? SerialId { get; private init; }
    public CustomerId? OwnerId { get; private init; }
    public Customer? Owner { get; private init; }
    public CustomerId? DealerId { get; private init; }
    public Customer? Dealer { get; private init; }
    public string? DeviceType { get; private init; }
    public string? PanelModel { get; private init; }
    public string? PanelSerialNumber { get; private init; }
    public string? WarrantyCardNumber { get; private init; }
    public DateTimeOffset? DateOfPurchase { get; private init; }
    public string? InvoiceNumber { get; private init; }
    public decimal? PurchasePrice { get; private init; }
    public bool IsUnrepairable { get; private init; }
    public DateTimeOffset? DateOfDemandForCompensation { get; private init; }
    public string? DemanderFullName { get; private init; }
    public HashSet<Order> Orders { get; private init; }

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
        HashSet<Order> orders,
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
        Orders = orders;
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
            created.CreatedBy,
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
        HashSet<Order>? orders,
        bool? isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        AppUserId createdBy,
        DateTimeOffset? createdAt,
        Func<ProductCreatedEvent, CancellationToken, Task<Product>> create,
        CancellationToken ct = default)
    {
        var productCreatedEvent = new ProductCreatedEvent(
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

        return await create(productCreatedEvent, ct);
    }

    public Product UpdateBrand(string? brand, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<ProductBrandChangedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrWhiteSpace(brand) && brand != Brand;
        if (!shouldUpdate) return this;
        
        var @event = new ProductBrandChangedEvent(
            Id,
            brand!,
            updateBy,
            updateAt);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product UpdateModel(string? model, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<ProductModelChangedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrWhiteSpace(model) && model != Model;
        if (!shouldUpdate) return this;
        
        var @event = new ProductModelChangedEvent(
            Id,
            model!,
            updateBy,
            updateAt);
        
        append(@event);
        return Apply(@event);
    }

    public Product UpdateDeviceType(string? deviceType, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<ProductDeviceTypeChangedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrWhiteSpace(deviceType) && deviceType != DeviceType;
        if (!shouldUpdate) return this;
        
        var @event = new ProductDeviceTypeChangedEvent(
            Id,
            deviceType,
            updateBy,
            updateAt);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product UpdatePanel(string? panelModel, string? panelSerialNumber, AppUserId updateBy, 
        DateTimeOffset? updateAt, Action<ProductPanelChangedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrWhiteSpace(panelModel) && panelModel != PanelModel
                           || !string.IsNullOrWhiteSpace(panelSerialNumber) && panelSerialNumber != PanelSerialNumber;
        
        if (!shouldUpdate) return this;
        
        var @event = new ProductPanelChangedEvent(
            Id,
            panelModel ?? PanelModel,
            panelSerialNumber ?? PanelSerialNumber,
            updateBy,
            updateAt);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product UpdateWarrantyCardNumber(string? warrantyCardNumber, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductWarrantyCardNumberChangedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrWhiteSpace(warrantyCardNumber) && warrantyCardNumber != WarrantyCardNumber;
        if (!shouldUpdate) return this;
        
        var @event = new ProductWarrantyCardNumberChangedEvent(
            Id,
            warrantyCardNumber,
            updateBy,
            updateAt);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product UpdatePurchaseData(DateTimeOffset? dateOfPurchase, string? invoiceNumber, decimal? purchasePrice, 
        AppUserId updateBy, DateTimeOffset? updateAt, Action<ProductPurchaseDataChangedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = dateOfPurchase is not null && dateOfPurchase != DateOfPurchase
                       || !string.IsNullOrWhiteSpace(invoiceNumber) && invoiceNumber != InvoiceNumber
                       || purchasePrice is not null && purchasePrice != PurchasePrice;
        if (!shouldUpdate) return this;
        
        var @event = new ProductPurchaseDataChangedEvent(
            Id,
            dateOfPurchase ?? DateOfPurchase,
            invoiceNumber ?? InvoiceNumber,
            purchasePrice ?? PurchasePrice,
            updateBy,
            updateAt);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product UpdateUnrepairable(bool? isUnrepairable, DateTimeOffset? dateOfDemandForCompensation, 
        string? demanderFullName, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductUnrepairableEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = isUnrepairable is not null && isUnrepairable != IsUnrepairable
                       || dateOfDemandForCompensation is not null && dateOfDemandForCompensation != DateOfDemandForCompensation
                       || !string.IsNullOrWhiteSpace(demanderFullName) && demanderFullName != DemanderFullName;
        if (!shouldUpdate) return this;
        
        var @event = new ProductUnrepairableEvent(
            Id,
            isUnrepairable ?? IsUnrepairable,
            dateOfDemandForCompensation ?? DateOfDemandForCompensation,
            demanderFullName ?? DemanderFullName,
            updateBy,
            updateAt);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product AddOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductOrderAddedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = orders is not null && !orders.SetEquals(Orders);
        if (!shouldUpdate) return this;
        
        var product = this;
        var ordersToAdd = orders!.Except(Orders);
        foreach (var orderId in ordersToAdd)
        {
            var @event = new ProductOrderAddedEvent(
                Id,
                orderId,
                updateBy,
                updateAt);

            append(@event);
            product = Apply(@event);
        }

        return product;
    }
    
    public Product RemoveOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductOrderRemovedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = orders is not null && !orders.SetEquals(Orders);
        if (!shouldUpdate) return this;
        
        var product = this;
        var ordersToRemove = Orders.Except(orders!);
        foreach (var orderId in ordersToRemove)
        {
            var @event = new ProductOrderRemovedEvent(
                Id,
                orderId,
                updateBy,
                updateAt);
            
            append(@event);
            product = Apply(@event);
        }
        
        return product;
    }
    
    public Product UpdateOwner(CustomerId ownerId, string? ownerName, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductOwnerChangedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = ownerId != OwnerId;
        if (!shouldUpdate) return this;
        
        var @event = new ProductOwnerChangedEvent(
            Id,
            ownerId.Value == string.Empty ? null : ownerId,
            ownerName,
            updateBy,
            updateAt);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product UpdateDealer(CustomerId dealerId, string? ownerName, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductDealerChangedEvent> append, out bool shouldUpdate)
    {
        shouldUpdate = dealerId != DealerId;
        if (!shouldUpdate) return this;
        
        var @event = new ProductDealerChangedEvent(
            Id,
            dealerId.Value == string.Empty ? null : dealerId,
            ownerName,
            updateBy,
            updateAt);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product Activate(AppUserId activateBy, Action<ProductActivatedEvent> append)
    {
        if (IsActive) return this;
        
        var @event = new ProductActivatedEvent(Id, activateBy);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product Deactivate(AppUserId deactivateBy, Action<ProductDeactivatedEvent> append)
    {
        if (!IsActive) return this;
        
        var @event = new ProductDeactivatedEvent(Id, deactivateBy);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product Delete(AppUserId deleteBy, Action<ProductDeletedEvent> append)
    {
        if (IsDeleted) return this;
        
        var @event = new ProductDeletedEvent(Id, deleteBy);
        
        append(@event);
        return Apply(@event);
    }
    
    public Product Undelete(AppUserId undeleteBy, Action<ProductUndeletedEvent> append)
    {
        if (!IsDeleted) return this;
        
        var @event = new ProductUndeletedEvent(Id, undeleteBy);
        
        append(@event);
        return Apply(@event);
    }

    public Product UpdateBrand(string? brand, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductBrandChangedEvent> append)
        => UpdateBrand(brand, updateBy, updateAt, append, out _);
    public Product UpdateModel(string? model, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<ProductModelChangedEvent> append)
        => UpdateModel(model, updateBy, updateAt, append, out _);
    public Product UpdateDeviceType(string? deviceType, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<ProductDeviceTypeChangedEvent> append)
        => UpdateDeviceType(deviceType, updateBy, updateAt, append, out _);
    public Product UpdatePanel(string? panelModel, string? panelSerialNumber, AppUserId updateBy, 
        DateTimeOffset? updateAt, Action<ProductPanelChangedEvent> append)
        => UpdatePanel(panelModel, panelSerialNumber, updateBy, updateAt, append, out _);
    public Product UpdateWarrantyCardNumber(string? warrantyCardNumber, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductWarrantyCardNumberChangedEvent> append)
        => UpdateWarrantyCardNumber(warrantyCardNumber, updateBy, updateAt, append, out _);
    public Product UpdatePurchaseData(DateTimeOffset? dateOfPurchase, string? invoiceNumber, decimal? purchasePrice, 
        AppUserId updateBy, DateTimeOffset? updateAt, Action<ProductPurchaseDataChangedEvent> append)
        => UpdatePurchaseData(dateOfPurchase, invoiceNumber, purchasePrice, updateBy, updateAt, append, out _);
    public Product UpdateUnrepairable(bool? isUnrepairable, DateTimeOffset? dateOfDemandForCompensation, 
        string? demanderFullName, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductUnrepairableEvent> append) => UpdateUnrepairable(isUnrepairable, 
        dateOfDemandForCompensation, demanderFullName, updateBy, updateAt, append, out _);
    public Product AddOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductOrderAddedEvent> append) 
        => AddOrders(orders, updateBy, updateAt, append, out _);
    public Product RemoveOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductOrderRemovedEvent> append)
        => RemoveOrders(orders, updateBy, updateAt, append, out _);
    public Product UpdateOwner(CustomerId ownerId, string? ownerName, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<ProductOwnerChangedEvent> append) 
        => UpdateOwner(ownerId, ownerName, updateBy, updateAt, append, out _);
    public Product UpdateDealer(CustomerId dealerId, string? ownerName, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<ProductDealerChangedEvent> append)
        => UpdateDealer(dealerId, ownerName, updateBy, updateAt, append, out _);
    
    public Product AddOwner(Customer? owner)
    {
        if (owner is null || owner == Owner) return this;
        return this with
        {
            Owner = owner
        };
    }
    
    public Product AddDealer(Customer? dealer)
    {
        if (dealer is null || dealer == Dealer) return this;
        return this with
        {
            Dealer = dealer
        };
    }
    
    public Product Apply(ProductBrandChangedEvent changed) =>
        this with
        {
            Brand = changed.Brand,
            LastModifiedAt = changed.BrandChangedAt,
            LastModifiedBy = changed.BrandChangedBy
        };
    
    public Product Apply(ProductModelChangedEvent changed) =>
        this with
        {
            Model = changed.Model,
            LastModifiedAt = changed.ModelChangedAt,
            LastModifiedBy = changed.ModelChangedBy
        };
    
    public Product Apply(ProductOwnerChangedEvent changed) =>
        this with
        {
            OwnerId = changed.OwnerId,
            LastModifiedAt = changed.OwnerChangedAt,
            LastModifiedBy = changed.OwnerChangedBy
        };
    
    public Product Apply(ProductDealerChangedEvent changed) =>
        this with
        {
            DealerId = changed.DealerId,
            LastModifiedAt = changed.DealerChangedAt,
            LastModifiedBy = changed.DealerChangedBy
        };
    
    public Product Apply(ProductOrderAddedEvent added)
    {
        Orders.Add(added.Order);
        return this with
        {
            LastModifiedAt = added.OrderAddedAt,
            LastModifiedBy = added.OrderAddedBy
        };
    }

    public Product Apply(ProductOrderRemovedEvent removed)
    {
        Orders.Remove(removed.Order);
        return this with
        {
            LastModifiedAt = removed.OrderRemovedAt,
            LastModifiedBy = removed.OrderRemovedBy
        };
    }
    
    public Product Apply(ProductDeviceTypeChangedEvent changed) =>
        this with
        {
            DeviceType = changed.DeviceType,
            LastModifiedAt = changed.DeviceTypeChangedAt,
            LastModifiedBy = changed.DeviceTypeChangedBy
        };
    
    public Product Apply(ProductPanelChangedEvent changed) =>
        this with
        {
            PanelModel = changed.PanelModel,
            PanelSerialNumber = changed.PanelSerialNumber,
            LastModifiedAt = changed.PanelChangedAt,
            LastModifiedBy = changed.PanelChangedBy
        };
    
    public Product Apply(ProductWarrantyCardNumberChangedEvent changed) =>
        this with
        {
            WarrantyCardNumber = changed.WarrantyCardNumber,
            LastModifiedAt = changed.WarrantyCardNumberChangedAt,
            LastModifiedBy = changed.WarrantyCardNumberChangedBy
        };

    
    public Product Apply(ProductPurchaseDataChangedEvent changed) =>
        this with
        {
            DateOfPurchase = changed.DateOfPurchase,
            InvoiceNumber = changed.InvoiceNumber,
            PurchasePrice = changed.PurchasePrice,
            LastModifiedAt = changed.DateOfPurchaseChangedAt,
            LastModifiedBy = changed.DateOfPurchaseChangedBy
        };
    
    public Product Apply(ProductUnrepairableEvent unrepairable) =>
        this with
        {
            IsUnrepairable = unrepairable.IsUnrepairable,
            DateOfDemandForCompensation = unrepairable.DateOfDemandForCompensation,
            DemanderFullName = unrepairable.DemanderFullName,
            LastModifiedAt = unrepairable.UnrepairableAt,
            LastModifiedBy = unrepairable.UnrepairableBy
        };
    
    public Product Apply(ProductActivatedEvent activated) =>
        this with
        {
            IsActive = true,
            LastModifiedAt = activated.ActivatedAt,
            LastModifiedBy = activated.ActivatedBy
        };
    
    public Product Apply(ProductDeactivatedEvent deactivated) =>
        this with
        {
            IsActive = false,
            LastModifiedAt = deactivated.DeactivatedAt,
            LastModifiedBy = deactivated.DeactivatedBy
        };
    
    public Product Apply(ProductDeletedEvent deleted) =>
        this with
        {
            IsActive = false,
            IsDeleted = true,
            LastModifiedAt = deleted.DeletedAt,
            LastModifiedBy = deleted.DeletedBy
        };
    
    public Product Apply(ProductUndeletedEvent undeleted) =>
        this with
        {
            IsActive = true,
            IsDeleted = false,
            LastModifiedAt = undeleted.UndeletedAt,
            LastModifiedBy = undeleted.UndeletedBy
        };


#pragma warning disable CS8618
    private Product() { }
#pragma warning restore CS8618
}