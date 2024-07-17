using Consumer.Domain.Common.Entities;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;
using Marten.Events;
using Marten.Events.Projections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Consumer.Infrastructure.Common.Persistence.Projections;

public class FlatTablesProjection : IProjection
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Dictionary<Type, Func<ConsumerDbContext, object, Dictionary<string, Customer>, Dictionary<string, Product>, CancellationToken, ValueTask>> _eventHandlerDefinitions;

    public FlatTablesProjection(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _eventHandlerDefinitions = new Dictionary<Type, Func<ConsumerDbContext, object, Dictionary<string, Customer>, Dictionary<string, Product>, CancellationToken, ValueTask>>
        {
            { typeof(CustomerCreatedEvent), async (db, e, _, cache, ct) => await HandleCustomerCreatedEvent(db, (CustomerCreatedEvent)e, cache, ct) },
            { typeof(CustomerNameChangedEvent), async (db, e, cache, _, ct) => await HandleCustomerEvent(db, (CustomerNameChangedEvent)e, cache, ct) },
            { typeof(CustomerAddressChangedEvent), async (db, e, cache, _, ct) => await HandleCustomerEvent(db, (CustomerAddressChangedEvent)e, cache, ct) },
            { typeof(CustomerPhoneNumberChangedEvent), async (db, e, cache, _, ct) => await HandleCustomerEvent(db, (CustomerPhoneNumberChangedEvent)e, cache, ct) },
            { typeof(CustomerRoleChangedEvent), async (db, e, cache, _, ct) => await HandleCustomerEvent(db, (CustomerRoleChangedEvent)e, cache, ct) },
            { typeof(CustomerProductAddedEvent), async (db, e, cache, _, ct) => await HandleCustomerProductAddedEvent(db, (CustomerProductAddedEvent)e, cache, ct) },
            { typeof(CustomerProductRemovedEvent), async (db, e, cache, _, ct) => await HandleCustomerProductRemovedEvent(db, (CustomerProductRemovedEvent)e, cache, ct) },
            { typeof(CustomerOrderAddedEvent), async (db, e, cache, _, ct) => await HandleCustomerOrderAddedEvent(db, (CustomerOrderAddedEvent)e, cache, ct) },
            { typeof(CustomerOrderRemovedEvent), async (db, e, cache, _, ct) => await HandleCustomerOrderRemovedEvent(db, (CustomerOrderRemovedEvent)e, cache, ct) },
            { typeof(CustomerActivatedEvent), async (db, e, cache, _, ct) => await HandleCustomerEvent(db, (CustomerActivatedEvent)e, cache, ct) },
            { typeof(CustomerDeactivatedEvent), async (db, e, cache, _, ct) => await HandleCustomerEvent(db, (CustomerDeactivatedEvent)e, cache, ct) },
            { typeof(CustomerDeletedEvent), async (db, e, cache, _, ct) => await HandleCustomerEvent(db, (CustomerDeletedEvent)e, cache, ct) },
            { typeof(CustomerUndeletedEvent), async (db, e, cache, _, ct) => await HandleCustomerEvent(db, (CustomerUndeletedEvent)e, cache, ct) },
            { typeof(ProductCreatedEvent), async (db, e, cache, _, ct) => await HandleProductCreatedEvent(db, (ProductCreatedEvent)e, cache, ct) },
            { typeof(ProductBrandChangedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductBrandChangedEvent)e, cache, ct) },
            { typeof(ProductModelChangedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductModelChangedEvent)e, cache, ct) },
            { typeof(ProductOwnerChangedEvent), async (db, e, _, cache, ct) => await HandleProductOwnerChangedEvent(db, (ProductOwnerChangedEvent)e, cache, ct) },
            { typeof(ProductDealerChangedEvent), async (db, e, _, cache, ct) => await HandleProductDealerChangedEvent(db, (ProductDealerChangedEvent)e, cache, ct) },
            { typeof(ProductOrderAddedEvent), async (db, e, _, cache, ct) => await HandleProductOrderAddedEvent(db, (ProductOrderAddedEvent)e, cache, ct) },
            { typeof(ProductOrderRemovedEvent), async (db, e, _, cache, ct) => await HandleProductOrderRemovedEvent(db, (ProductOrderRemovedEvent)e, cache, ct) },
            { typeof(ProductDeviceTypeChangedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductDeviceTypeChangedEvent)e, cache, ct) },
            { typeof(ProductPanelChangedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductPanelChangedEvent)e, cache, ct) },
            { typeof(ProductWarrantyCardNumberChangedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductWarrantyCardNumberChangedEvent)e, cache, ct) },
            { typeof(ProductPurchaseDataChangedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductPurchaseDataChangedEvent)e, cache, ct) },
            { typeof(ProductUnrepairableEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductUnrepairableEvent)e, cache, ct) },
            { typeof(ProductActivatedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductActivatedEvent)e, cache, ct) },
            { typeof(ProductDeactivatedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductDeactivatedEvent)e, cache, ct) },
            { typeof(ProductDeletedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductDeletedEvent)e, cache, ct) },
            { typeof(ProductUndeletedEvent), async (db, e, _, cache, ct) => await HandleProductEvent(db, (ProductUndeletedEvent)e, cache, ct) },
        };
    }

    public void Apply(Marten.IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
    {
        ApplyAsync(operations, streams, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task ApplyAsync(Marten.IDocumentOperations operations, IReadOnlyList<StreamAction> streams, CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConsumerDbContext>();
        var customerCache = new Dictionary<string, Customer>();
        var productCache = new Dictionary<string, Product>();

        var eventHandlers = _eventHandlerDefinitions.ToDictionary(
            kvp => kvp.Key,
            kvp => new Func<object, ValueTask>(e => kvp.Value(dbContext, e, customerCache, productCache, ct))
        );
        
        foreach (var stream in streams)
        {
            var productCreatedEvents = new List<object>();
            var customerCreatedEvents = new List<object>();
            var otherEvents = new List<object>();

            foreach (var @event in stream.Events)
            {
                var eventType = @event.Data.GetType();
                if (eventType == typeof(ProductCreatedEvent))
                {
                    productCreatedEvents.Add(@event.Data);
                }
                else if (eventType == typeof(CustomerCreatedEvent))
                {
                    customerCreatedEvents.Add(@event.Data);
                }
                else
                {
                    otherEvents.Add(@event.Data);
                }
            }
            
            await HandleEvents(productCreatedEvents, eventHandlers);
            await HandleEvents(customerCreatedEvents, eventHandlers);
            await HandleEvents(otherEvents, eventHandlers);
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private async Task HandleEvents(IEnumerable<Object> events, Dictionary<Type, Func<object, ValueTask>> eventHandlers)
    {
        foreach (var @event in events)
        {
            var eventType = @event.GetType();
            if (eventHandlers.TryGetValue(eventType, out var handler))
            {
                await handler(@event);
            }
            else
            {
                throw new InvalidOperationException($"No handler found for event type {eventType}");
            }
        }
    }

    private async ValueTask HandleCustomerEvent(ConsumerDbContext dbContext, CustomerEvent @event, 
        Dictionary<string, Customer> customerCache, CancellationToken ct = default)
    {
        if (!customerCache.TryGetValue(@event.CustomerId.Value, out var customer))
        {
            customer = await dbContext.Customers.FirstAsync(p => p.Id == @event.CustomerId, ct);
            customerCache[@event.CustomerId.Value] = customer;
        }
        customer.Apply(@event);
    }

    private async ValueTask HandleProductEvent(ConsumerDbContext dbContext, ProductEvent @event, 
        Dictionary<string, Product> productCache, CancellationToken ct = default)
    {
        if (!productCache.TryGetValue(@event.ProductId.Value, out var product))
        {
            product = await dbContext.Products.FirstAsync(p => p.Id == @event.ProductId, ct);
            productCache[@event.ProductId.Value] = product;
        }
        product.Apply(@event);
    }

    private async ValueTask HandleCustomerCreatedEvent(ConsumerDbContext dbContext, CustomerCreatedEvent @event, 
        Dictionary<string, Product> productCache, CancellationToken ct = default)
    {
        var customer = Customer.Create(@event);
        dbContext.Customers.Add(customer);

        foreach (var productId in @event.ProductIds)
        {
            if (!productCache.TryGetValue(productId.Value, out var product))
            {
                product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId, ct);
                if (product is null) break;
                productCache[productId.Value] = product;
            }

            var customerProduct = new CustomerProduct
            {
                CustomerId = @event.CustomerId,
                ProductId = productId
            };

            await AddCustomerProduct(dbContext, customerProduct, ct);
        }

        foreach (var order in @event.Orders)
        {
            var customerOrder = new CustomerOrder
            {
                CustomerId = @event.CustomerId,
                OrderId = order.OrderId,
                CentreId = order.CentreId
            };

            await AddCustomerOrder(dbContext, customerOrder, ct);
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private async ValueTask HandleCustomerProductAddedEvent(ConsumerDbContext dbContext, CustomerProductAddedEvent @event,
        Dictionary<string, Customer> customerCache, CancellationToken ct = default)
    {
        var customerProduct = new CustomerProduct { CustomerId = @event.CustomerId, ProductId = @event.ProductId };
        await AddCustomerProduct(dbContext, customerProduct, ct);
        await HandleCustomerEvent(dbContext, @event, customerCache, ct);
    }

    private async ValueTask HandleCustomerProductRemovedEvent(ConsumerDbContext dbContext, CustomerProductRemovedEvent @event,
        Dictionary<string, Customer> customerCache, CancellationToken ct = default)
    {
        var customerProduct = new CustomerProduct { CustomerId = @event.CustomerId, ProductId = @event.ProductId };
        await RemoveCustomerProduct(dbContext, customerProduct, ct);
        await HandleCustomerEvent(dbContext, @event, customerCache, ct);
    }
    
    private async ValueTask HandleCustomerOrderAddedEvent(ConsumerDbContext dbContext, CustomerOrderAddedEvent @event,
        Dictionary<string, Customer> customerCache, CancellationToken ct = default)
    {
        var customerOrder = new CustomerOrder
        {
            CustomerId = @event.CustomerId,
            OrderId = @event.Order.OrderId,
            CentreId = @event.Order.CentreId
        };
        await AddCustomerOrder(dbContext, customerOrder, ct);
        await HandleCustomerEvent(dbContext, @event, customerCache, ct);
    }

    private async ValueTask HandleCustomerOrderRemovedEvent(ConsumerDbContext dbContext, CustomerOrderRemovedEvent @event, 
        Dictionary<string, Customer> customerCache, CancellationToken ct = default)
    {
        var customerOrder = new CustomerOrder { CustomerId = @event.CustomerId, OrderId = @event.Order.OrderId };
        await RemoveCustomerOrder(dbContext, customerOrder, ct);
        await HandleCustomerEvent(dbContext, @event, customerCache, ct);
    }
    
    private async ValueTask HandleProductCreatedEvent(ConsumerDbContext dbContext, ProductCreatedEvent @event, 
        Dictionary<string, Customer> customerCache, CancellationToken ct = default)
    {
        var product = Product.Create(@event);
        dbContext.Products.Add(product);

        if (product.OwnerId is not null)
        {
            var ownerId = product.OwnerId;
            if (!customerCache.TryGetValue(ownerId.Value, out var owner))
            {
                owner = await dbContext.Customers.FirstOrDefaultAsync(p => p.Id == ownerId, ct);
                if (owner is not null) customerCache[ownerId.Value] = owner;
            }
            if (owner is not null)
            {
                var customerProduct = new CustomerProduct
                {
                    CustomerId = ownerId,
                    ProductId = product.Id
                };

                await AddCustomerProduct(dbContext, customerProduct, ct);
            }
        }
        
        if (product.DealerId is not null)
        {
            var dealerId = product.DealerId;
            if (!customerCache.TryGetValue(dealerId.Value, out var dealer))
            {
                dealer = await dbContext.Customers.FirstOrDefaultAsync(p => p.Id == dealerId, ct);
                if (dealer is not null) customerCache[dealerId.Value] = dealer;
            }
            if (dealer is not null)
            {
                customerCache[dealerId.Value] = dealer;
                var customerProduct = new CustomerProduct
                {
                    CustomerId = dealerId,
                    ProductId = product.Id
                };

                await AddCustomerProduct(dbContext, customerProduct, ct);
            }
        }

        foreach (var order in @event.Orders)
        {
            var productOrder = new ProductOrder
            {
                ProductId = @event.ProductId,
                OrderId = order.OrderId,
                CentreId = order.CentreId
            };

            await AddProductOrder(dbContext, productOrder, ct);
        }

        await dbContext.SaveChangesAsync(ct);
    }

    private async ValueTask HandleProductOwnerChangedEvent(ConsumerDbContext dbContext, ProductOwnerChangedEvent @event,
        Dictionary<string, Product> productCache, CancellationToken ct = default)
    {
        if (!productCache.TryGetValue(@event.ProductId.Value, out var product))
        {
            product = await dbContext.Products.FirstAsync(p => p.Id == @event.ProductId, ct);
            productCache[@event.ProductId.Value] = product;
        }
        
        if (product.OwnerId is not null)
        {
            var customerProduct = new CustomerProduct { ProductId = @event.ProductId, CustomerId = product.OwnerId };
            await RemoveCustomerProduct(dbContext, customerProduct, ct);
        }

        if (@event.OwnerId is not null)
        {
            var customerProduct = new CustomerProduct { ProductId = @event.ProductId, CustomerId = @event.OwnerId };
            await AddCustomerProduct(dbContext, customerProduct, ct);
        }
        
        await HandleProductEvent(dbContext, @event, productCache, ct);
    }
    
    private async ValueTask HandleProductDealerChangedEvent(ConsumerDbContext dbContext, ProductDealerChangedEvent @event,
        Dictionary<string, Product> productCache, CancellationToken ct = default)
    {
        if (!productCache.TryGetValue(@event.ProductId.Value, out var product))
        {
            product = await dbContext.Products.FirstAsync(p => p.Id == @event.ProductId, ct);
            productCache[@event.ProductId.Value] = product;
        }
        
        if (product.DealerId is not null)
        {
            var customerProduct = new CustomerProduct { ProductId = @event.ProductId, CustomerId = product.DealerId };
            await RemoveCustomerProduct(dbContext, customerProduct, ct);
        }

        if (@event.DealerId is not null)
        {
            var customerProduct = new CustomerProduct { ProductId = @event.ProductId, CustomerId = @event.DealerId };
            await AddCustomerProduct(dbContext, customerProduct, ct);
        }
        
        await HandleProductEvent(dbContext, @event, productCache, ct);
    }
    
    private async ValueTask HandleProductOrderAddedEvent(ConsumerDbContext dbContext, ProductOrderAddedEvent @event,
        Dictionary<string, Product> productCache, CancellationToken ct = default)
    {
        var productOrder = new ProductOrder
        {
            ProductId = @event.ProductId,
            OrderId = @event.Order.OrderId,
            CentreId = @event.Order.CentreId
        };
        await AddProductOrder(dbContext, productOrder, ct);
        await HandleProductEvent(dbContext, @event, productCache, ct);
    }

    private async ValueTask HandleProductOrderRemovedEvent(ConsumerDbContext dbContext, ProductOrderRemovedEvent @event,
        Dictionary<string, Product> productCache, CancellationToken ct = default)
    {
        var productOrder = new ProductOrder { ProductId = @event.ProductId, OrderId = @event.Order.OrderId };
        await RemoveProductOrder(dbContext, productOrder, ct);
        await HandleProductEvent(dbContext, @event, productCache, ct);
    }
    
    private async ValueTask AddCustomerProduct(ConsumerDbContext dbContext, CustomerProduct customerProduct, CancellationToken ct = default)
    {
        var tracked = dbContext.CustomerProducts.Local
            .Any(cp => cp.CustomerId == customerProduct.CustomerId && cp.ProductId == customerProduct.ProductId);

        if (tracked)
        {
            return;
        }

        var exists = await dbContext.CustomerProducts.AsNoTracking()
            .AnyAsync(cp => cp.CustomerId == customerProduct.CustomerId && cp.ProductId == customerProduct.ProductId, ct);

        if (!exists)
        {
            dbContext.CustomerProducts.Add(customerProduct);
        }
    }
    
    private async ValueTask RemoveCustomerProduct(ConsumerDbContext dbContext, CustomerProduct customerProduct, CancellationToken ct = default)
    {
        var tracked = dbContext.CustomerProducts.Local
            .Any(cp => cp.CustomerId == customerProduct.CustomerId && cp.ProductId == customerProduct.ProductId);
        
        if (tracked)
        {
            dbContext.CustomerProducts.Remove(customerProduct);
            return;
        }
        
        var exists = await dbContext.CustomerProducts.AsNoTracking()
            .AnyAsync(cp => cp.CustomerId == customerProduct.CustomerId && cp.ProductId == customerProduct.ProductId, ct);

        if (exists)
        {
            dbContext.CustomerProducts.Remove(customerProduct);
        }
    }
    
    private async ValueTask AddCustomerOrder(ConsumerDbContext dbContext, CustomerOrder customerOrder, CancellationToken ct = default)
    {
        var tracked = dbContext.CustomerOrders.Local
            .Any(co => co.CustomerId == customerOrder.CustomerId && co.OrderId == customerOrder.OrderId);

        if (tracked)
        {
            return;
        }
        
        var exists = await dbContext.CustomerOrders.AsNoTracking()
            .AnyAsync(co => co.CustomerId == customerOrder.CustomerId && co.OrderId == customerOrder.OrderId, ct);

        if (!exists)
        {
            dbContext.CustomerOrders.Add(customerOrder);
        }
    }
    
    private async ValueTask RemoveCustomerOrder(ConsumerDbContext dbContext, CustomerOrder customerOrder, CancellationToken ct = default)
    {
        var tracked = dbContext.CustomerOrders.Local
            .Any(co => co.CustomerId == customerOrder.CustomerId && co.OrderId == customerOrder.OrderId);
        
        if (tracked)
        {
            dbContext.CustomerOrders.Remove(customerOrder);
            return;
        }
        
        var exists = await dbContext.CustomerOrders.AsNoTracking()
            .AnyAsync(co => co.CustomerId == customerOrder.CustomerId && co.OrderId == customerOrder.OrderId, ct);

        if (exists)
        {
            dbContext.CustomerOrders.Remove(customerOrder);
        }
    }
    
    private async ValueTask AddProductOrder(ConsumerDbContext dbContext, ProductOrder productOrder, CancellationToken ct = default)
    {
        var tracked = dbContext.ProductOrders.Local
            .Any(po => po.ProductId == productOrder.ProductId && po.OrderId == productOrder.OrderId);

        if (tracked)
        {
            return;
        }
        
        var exists = await dbContext.ProductOrders.AsNoTracking()
            .AnyAsync(po => po.ProductId == productOrder.ProductId && po.OrderId == productOrder.OrderId, ct);

        if (!exists)
        {
            dbContext.ProductOrders.Add(productOrder);
        }
    }
    
    private async ValueTask RemoveProductOrder(ConsumerDbContext dbContext, ProductOrder productOrder, CancellationToken ct = default)
    {
        var tracked = dbContext.ProductOrders.Local
            .Any(po => po.ProductId == productOrder.ProductId && po.OrderId == productOrder.OrderId);
        
        if (tracked)
        {
            dbContext.ProductOrders.Remove(productOrder);
            return;
        }
        
        var exists = await dbContext.ProductOrders.AsNoTracking()
            .AnyAsync(po => po.ProductId == productOrder.ProductId && po.OrderId == productOrder.OrderId, ct);

        if (exists)
        {
            dbContext.ProductOrders.Remove(productOrder);
        }
    }
}