using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Common.Models;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.Entities;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products;

public sealed record Product : AggregateRoot<ProductId, string>
{
    public string Brand { get; set; }
    public string Model { get; set; }
    public SerialId? SerialId { get; set; }
    public CustomerId? OwnerId { get; set; }
    public Customer? Owner { get; set; }
    public CustomerId? DealerId { get; set; }
    public Customer? Dealer { get; set; }
    public string? DeviceType { get; set; }
    public string? PanelModel { get; set; }
    public string? PanelSerialNumber { get; set; }
    public string? WarrantyCardNumber { get; set; }
    public DateTimeOffset? DateOfPurchase { get; set; }
    public string? InvoiceNumber { get; set; }
    public decimal? PurchasePrice { get; set; }
    public bool IsUnrepairable { get; set; }
    public DateTimeOffset? DateOfDemandForCompensation { get; set; }
    public string? DemanderFullName { get; set; }
    public HashSet<Order> Orders { get; set; }

    private Product(
        ProductId id,
        string brand,
        string model,
        SerialId? serialId,
        CustomerId? ownerId,
        CustomerId? dealerId,
        HashSet<Order>? orders,
        string? deviceType,
        string? panelModel,
        string? panelSerialNumber,
        string? warrantyCardNumber,
        DateTimeOffset? dateOfPurchase,
        string? invoiceNumber,
        decimal? purchasePrice,
        bool? isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        DateTimeOffset? createdAt,
        AppUserId createdBy)
    {
        Id = id;
        Brand = brand;
        Model = model;
        SerialId = serialId;
        OwnerId = ownerId;
        DealerId = dealerId;
        Orders = orders ?? new();
        DeviceType = deviceType;
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        WarrantyCardNumber = warrantyCardNumber;
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        IsUnrepairable = isUnrepairable ?? false;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
        IsActive = true;
    }
    
    public static Product Create(ProductCreatedEvent created) =>
        new(created.ProductId,
            created.Brand,
            created.Model,
            created.SerialId,
            created.OwnerId,
            created.DealerId,
            created.Orders,
            created.DeviceType,
            created.PanelModel,
            created.PanelSerialNumber,
            created.WarrantyCardNumber,
            created.DateOfPurchase,
            created.InvoiceNumber,
            created.PurchasePrice,
            created.IsUnrepairable,
            created.DateOfDemandForCompensation,
            created.DemanderFullName,
            created.CreatedAt,
            created.CreatedBy);
    
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