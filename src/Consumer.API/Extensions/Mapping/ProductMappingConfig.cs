using Mapster;
using Consumer.Application.Products.Commands.Update;
using Consumer.Application.Products.Commands.Create;
using Consumer.API.Contract.V1.Products.Requests;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.API.Contract.V1.Products.Responses.Events;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.Entities;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.API.Extensions.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PostProductRequest, CreateProductCommand>()
            .Map(command => command.AppUserId, request => new AppUserId(request.PostBy))
            .Map(command => command.ProductId, request => new ProductId(request.ProductId))
            .Map(command => command.OwnerId, request => request.OwnerId == null ? 
                null : new CustomerId(request.OwnerId))
            .Map(command => command.DealerId, request => request.DealerId == null ? 
                null : new CustomerId(request.DealerId))
            .Map(command => command.SerialId, request => request.SerialId == null ? 
                null : new SerialId(request.SerialId.Value))
            .Map(command => command.Orders, request => request.Orders == null ? null : 
                request.Orders.Select(o => Order.Create(new OrderId(o.Item1), new CentreId(o.Item2))).ToHashSet())
            .Map(command => command.CreatedAt, request => request.PostAt);

        config.NewConfig<PatchProductRequest, UpdateProductCommand>()
            .Map(command => command.AppUserId, request => new AppUserId(request.PatchBy))
            .Map(command => command.OwnerId, request => request.OwnerId == null ? 
                null : new CustomerId(request.OwnerId))
            .Map(command => command.DealerId, request => request.DealerId == null ? 
                null : new CustomerId(request.DealerId))
            .Map(command => command.Orders, request => request.Orders == null ? null : 
                request.Orders.Select(o => Order.Create(new OrderId(o.Item1), new CentreId(o.Item2))).ToHashSet())
            .Map(command => command.UpdatedAt, request => request.PatchAt);

        config.NewConfig<Product, ProductForListingResponse>()
            .Map(response => response.ProductId, product => product.Id.Value)
            .Map(response => response.OwnerId, product => product.OwnerId == null ?
                null : product.OwnerId.Value)
            .Map(response => response.DealerId, product => product.DealerId == null ?
                null : product.DealerId.Value)
            .Map(response => response.SerialId,
                product => product.SerialId == null ? (int?)null : product.SerialId.Value);

        config.NewConfig<Product, ProductResponse>()
            .Map(response => response.ProductId, product => product.Id.Value)
            .Map(response => response.OwnerId, product => product.OwnerId == null ?
                null : product.OwnerId.Value)
            .Map(response => response.DealerId, product => product.DealerId == null ?
                null : product.DealerId.Value)
            .Map(response => response.SerialId, product => product.SerialId == null ?
                (int?)null : product.SerialId.Value)
            .Map(result => result.CreatedBy, product => product.CreatedBy.Value)
            .Map(result => result.LastModifiedBy,
                product => product.LastModifiedBy == null ? (Guid?)null : product.LastModifiedBy.Value);
        
        config.NewConfig<Order, ProductOrder>()
            .Map(response => response.OrderId, order => order.OrderId.Value)
            .Map(response => response.CentreId, order => order.CentreId.Value);
        
        config.NewConfig<ProductCreatedEvent, ProductCreated>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.SerialId, @event => @event.SerialId == null ? 
                (int?)null : @event.SerialId.Value)
            .Map(response => response.OwnerId, @event => @event.OwnerId == null ? 
                null : @event.OwnerId.Value)
            .Map(response => response.DealerId, @event => @event.DealerId == null ? 
                null : @event.DealerId.Value)
            .Map(response => response.Orders, @event => @event.Orders == null ? 
                null : @event.Orders.Select(x => new ProductOrder(x.OrderId.Value, x.CentreId.Value)))
            .Map(response => response.CreatedBy, @event => @event.CreatedBy.Value);
        
        config.NewConfig<ProductBrandChangedEvent, ProductBrandChanged>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.BrandChangedBy, @event => @event.BrandChangedBy.Value);
        
        config.NewConfig<ProductModelChangedEvent, ProductModelChanged>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.ModelChangedBy, @event => @event.ModelChangedBy.Value);
        
        config.NewConfig<ProductOwnerChangedEvent, ProductOwnerChanged>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.OwnerId, @event => @event.OwnerId == null ?
                null : @event.OwnerId.Value)
            .Map(response => response.OwnerChangedBy, @event => @event.OwnerChangedBy.Value);
        
        config.NewConfig<ProductDealerChangedEvent, ProductDealerChanged>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.DealerId, @event => @event.DealerId == null ?
                null : @event.DealerId.Value)
            .Map(response => response.DealerChangedBy, @event => @event.DealerChangedBy.Value);
        
        config.NewConfig<ProductOrderAddedEvent, ProductOrderAdded>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.Order.OrderId, @event => @event.Order.OrderId.Value)
            .Map(response => response.Order.CentreId, @event => @event.Order.CentreId.Value)
            .Map(response => response.OrderAddedBy, @event => @event.OrderAddedBy.Value);
        
        config.NewConfig<ProductOrderRemovedEvent, ProductOrderRemoved>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.Order.OrderId, @event => @event.Order.OrderId.Value)
            .Map(response => response.Order.CentreId, @event => @event.Order.CentreId.Value)
            .Map(response => response.OrderRemovedBy, @event => @event.OrderRemovedBy.Value);

        config.NewConfig<ProductDeviceTypeChangedEvent, ProductDeviceTypeChanged>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.DeviceTypeChangedBy, @event => @event.DeviceTypeChangedBy.Value);
        
        config.NewConfig<ProductPanelChangedEvent, ProductPanelChanged>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.PanelChangedBy, @event => @event.PanelChangedBy.Value);
        
        config.NewConfig<ProductWarrantyCardNumberChangedEvent, ProductWarrantyCardNumberChanged>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.WarrantyCardNumberChangedBy, @event => @event.WarrantyCardNumberChangedBy.Value);
        
        config.NewConfig<ProductPurchaseDataChangedEvent, ProductPurchaseDataChanged>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.DateOfPurchaseChangedBy, @event => @event.DateOfPurchaseChangedBy.Value);
        
        config.NewConfig<ProductUnrepairableEvent, ProductUnrepairable>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.UnrepairableBy, @event => @event.UnrepairableBy.Value);
        
        config.NewConfig<ProductActivatedEvent, ProductActivated>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.ActivatedBy, @event => @event.ActivatedBy.Value);
        
        config.NewConfig<ProductDeactivatedEvent, ProductDeactivated>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.DeactivatedBy, @event => @event.DeactivatedBy.Value);
        
        config.NewConfig<ProductDeletedEvent, ProductDeleted>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.DeletedBy, @event => @event.DeletedBy.Value);
        
        config.NewConfig<ProductUndeletedEvent, ProductUndeleted>()
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.UndeletedBy, @event => @event.UndeletedBy.Value);
    }
}