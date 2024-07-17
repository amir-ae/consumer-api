using Marten.Events.Projections.Flattened;
using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;
using Consumer.Infrastructure.Common.Extensions;

namespace Consumer.Infrastructure.Products.Projections;

public class ProductProjection : FlatTableProjection
{
    public ProductProjection() : base("Products", SchemaNameSource.EventSchema)
    {
        Options.TeardownDataOnRebuild = false;

        Table.AddColumn<string>(nameof(Product.Id).ToSnakeCase()).AsPrimaryKey();
        Table.AddColumn<string>(nameof(Product.AggregateId).ToSnakeCase());
        Table.AddColumn<string>(nameof(Product.Brand).ToSnakeCase());
        Table.AddColumn<string>(nameof(Product.Model).ToSnakeCase());
        Table.AddColumn<int>(nameof(Product.SerialId).ToSnakeCase()).AllowNulls();
        Table.AddColumn<string>(nameof(Product.OwnerId).ToSnakeCase());
        Table.AddColumn<string>(nameof(Product.DealerId).ToSnakeCase());
        Table.AddColumn<string>(nameof(Product.DeviceType).ToSnakeCase());
        Table.AddColumn<string>(nameof(Product.PanelModel).ToSnakeCase());
        Table.AddColumn<string>(nameof(Product.PanelSerialNumber).ToSnakeCase());
        Table.AddColumn<string>(nameof(Product.WarrantyCardNumber).ToSnakeCase());
        Table.AddColumn<DateTimeOffset>(nameof(Product.DateOfPurchase).ToSnakeCase()).AllowNulls();
        Table.AddColumn<string>(nameof(Product.InvoiceNumber).ToSnakeCase());
        Table.AddColumn<decimal>(nameof(Product.PurchasePrice).ToSnakeCase()).AllowNulls();
        //Table.AddColumn<int>(nameof(Product.IsUnrepairable).ToSnakeCase()).DefaultValue(0);
        Table.AddColumn<DateTimeOffset>(nameof(Product.DateOfDemandForCompensation).ToSnakeCase()).AllowNulls();
        Table.AddColumn<string>(nameof(Product.DemanderFullName).ToSnakeCase());
        Table.AddColumn<string>(nameof(Product.ProductOrders).ToSnakeCase());
        Table.AddColumn<DateTimeOffset>(nameof(Product.CreatedAt).ToSnakeCase());
        Table.AddColumn<Guid>(nameof(Product.CreatedBy).ToSnakeCase());
        Table.AddColumn<DateTimeOffset>(nameof(Product.LastModifiedAt).ToSnakeCase());
        Table.AddColumn<Guid>(nameof(Product.LastModifiedBy).ToSnakeCase());
        Table.AddColumn<int>(nameof(Product.Version).ToSnakeCase());
        //Table.AddColumn<int>(nameof(Product.IsActive).ToSnakeCase()).DefaultValue(1);
        //Table.AddColumn<int>(nameof(Product.IsDeleted).ToSnakeCase()).DefaultValue(0);

        /*Project<ProductCreatedEvent>(map =>
        {
            map.Map(x => x.Brand, nameof(Product.Brand).ToSnakeCase());
            map.Map(x => x.Model, nameof(Product.Model).ToSnakeCase());
            //map.Map(x => x.SerialIdValue, nameof(Product.SerialId).ToSnakeCase()).DefaultValue(0);
            map.Map(x => x.OwnerId!.Value, nameof(Product.OwnerId).ToSnakeCase());
            map.Map(x => x.DealerId!.Value, nameof(Product.DealerId).ToSnakeCase());
            map.Map(x => x.DeviceType, nameof(Product.DeviceType).ToSnakeCase());
            map.Map(x => x.PanelModel, nameof(Product.PanelModel).ToSnakeCase());
            map.Map(x => x.PanelSerialNumber, nameof(Product.PanelSerialNumber).ToSnakeCase());
            map.Map(x => x.WarrantyCardNumber, nameof(Product.WarrantyCardNumber).ToSnakeCase());
            
            //map.Map(x => x.DateOfPurchase, nameof(Product.DateOfPurchase).ToSnakeCase());
            map.Map(x => x.InvoiceNumber, nameof(Product.InvoiceNumber).ToSnakeCase());
            //map.Map(x => x.PurchasePrice, nameof(Product.PurchasePrice).ToSnakeCase());
            //map.Map(x => x.IsUnrepairable, nameof(Product.IsUnrepairable).ToSnakeCase());
            //map.Map(x => x.DateOfDemandForCompensation, nameof(Product.DateOfDemandForCompensation).ToSnakeCase());
            map.Map(x => x.DemanderFullName, nameof(Product.DemanderFullName).ToSnakeCase());
            map.Map(x => x.OrdersString, nameof(Product.ProductOrders).ToSnakeCase());
            map.Map(x => x.CreatedAt, nameof(Product.CreatedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.CreatedBy).ToSnakeCase());
            map.Map(x => x.ProductId.Value, nameof(Product.AggregateId).ToSnakeCase());
            map.SetValue(nameof(Product.Version).ToSnakeCase(), 1);
        });*/
        
        /*Project<ProductBrandChangedEvent>(map =>
        {
            map.Map(x => x.Brand, nameof(Product.Brand).ToSnakeCase());
            map.Map(x => x.BrandChangedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });
        
        Project<ProductModelChangedEvent>(map =>
        {
            map.Map(x => x.Model, nameof(Product.Model).ToSnakeCase());
            map.Map(x => x.ModelChangedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });*/
        
        Project<ProductDeviceTypeChangedEvent>(map =>
        {
            map.Map(x => x.DeviceType, nameof(Product.DeviceType).ToSnakeCase());
            map.Map(x => x.DeviceTypeChangedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });
        
        Project<ProductPanelChangedEvent>(map =>
        {
            map.Map(x => x.PanelModel, nameof(Product.PanelModel).ToSnakeCase());
            map.Map(x => x.PanelSerialNumber, nameof(Product.PanelSerialNumber).ToSnakeCase());
            map.Map(x => x.PanelChangedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });
        
        Project<ProductWarrantyCardNumberChangedEvent>(map =>
        {
            map.Map(x => x.WarrantyCardNumber, nameof(Product.WarrantyCardNumber).ToSnakeCase());
            map.Map(x => x.WarrantyCardNumberChangedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });
        
        /*Project<ProductPurchaseDataChangedEvent>(map =>
        {
            map.Map(x => x.DateOfPurchase, nameof(Product.DateOfPurchase).ToSnakeCase());
            map.Map(x => x.InvoiceNumber, nameof(Product.InvoiceNumber).ToSnakeCase());
            map.Map(x => x.PurchasePrice, nameof(Product.PurchasePrice).ToSnakeCase());
            map.Map(x => x.PurchaseDataChangedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });*/
        
        /*Project<ProductUnrepairableEvent>(map =>
        {
            map.Map(x => x.IsUnrepairable, nameof(Product.IsUnrepairable).ToSnakeCase());
            map.Map(x => x.DateOfDemandForCompensation, nameof(Product.DateOfDemandForCompensation).ToSnakeCase());
            map.Map(x => x.DemanderFullName, nameof(Product.DemanderFullName).ToSnakeCase());
            map.Map(x => x.UnrepairableAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });*/
        
        Project<ProductOrderAddedEvent>(map =>
        {
            map.Map(x => x.OrdersString, nameof(Product.ProductOrders).ToSnakeCase());
            map.Map(x => x.OrderAddedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });
        
        Project<ProductOrderRemovedEvent>(map =>
        {
            map.Map(x => x.OrdersString, nameof(Product.ProductOrders).ToSnakeCase());
            map.Map(x => x.OrderRemovedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });

        Project<ProductOwnerChangedEvent>(map =>
        {
            map.Map(x => x.OwnerId!.Value, nameof(Product.OwnerId).ToSnakeCase());
            map.Map(x => x.OwnerChangedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });
        
        Project<ProductDealerChangedEvent>(map =>
        {
            map.Map(x => x.DealerId!.Value, nameof(Product.DealerId).ToSnakeCase());
            map.Map(x => x.DealerChangedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });

        /*Project<ProductActivatedEvent>(map =>
        {
            map.SetValue(nameof(Product.IsActive).ToSnakeCase(), 1);
            map.Map(x => x.ActivatedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });
        
        Project<ProductDeactivatedEvent>(map =>
        {
            map.SetValue(nameof(Product.IsActive).ToSnakeCase(), 0);
            map.Map(x => x.DeactivatedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });
        
        Project<ProductDeletedEvent>(map =>
        {
            map.SetValue(nameof(Product.IsDeleted).ToSnakeCase(), 1);
            map.Map(x => x.DeletedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });
        
        Project<ProductUndeletedEvent>(map =>
        {
            map.SetValue(nameof(Product.IsDeleted).ToSnakeCase(), 0);
            map.Map(x => x.UndeletedAt, nameof(Product.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Product.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Product.Version).ToSnakeCase());
        });*/
    }
}