using Consumer.Domain.Common.Entities;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Products.Events;
using Marten.Events;
using Marten.Events.Projections;

namespace Consumer.Infrastructure.Common.Persistence.Projections;

public class LinkTablesProjection : IProjection
{
    private readonly ConsumerDbContext _dbContext;
    private bool _changesMade;

    public LinkTablesProjection(ConsumerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Apply(Marten.IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
    {
        ApplyAsync(operations, streams, CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task ApplyAsync(Marten.IDocumentOperations operations, IReadOnlyList<StreamAction> streams, CancellationToken ct)
    {
        foreach (var stream in streams)
        {
            foreach (var @event in stream.Events)
            {
                switch (@event.Data)
                {
                    case CustomerCreatedEvent createdEvent:
                        HandleCustomerCreatedEvent(createdEvent);
                        break;

                    case CustomerProductAddedEvent addedEvent:
                        HandleCustomerProductAddedEvent(addedEvent);
                        break;

                    case CustomerProductRemovedEvent removedEvent:
                        HandleCustomerProductRemovedEvent(removedEvent);
                        break;
                    
                    case CustomerOrderAddedEvent addedEvent:
                        HandleCustomerOrderAddedEvent(addedEvent);
                        break;

                    case CustomerOrderRemovedEvent removedEvent:
                        HandleCustomerOrderRemovedEvent(removedEvent);
                        break;
                    
                    case ProductCreatedEvent createdEvent:
                        HandleProductCreatedEvent(createdEvent);
                        break;

                    case ProductOrderAddedEvent addedEvent:
                        HandleProductOrderAddedEvent(addedEvent);
                        break;

                    case ProductOrderRemovedEvent removedEvent:
                        HandleProductOrderRemovedEvent(removedEvent);
                        break;
                }
            }
        }

        if (_changesMade)
        {
            await _dbContext.SaveChangesAsync(ct);
        }
    }

    private void HandleCustomerCreatedEvent(CustomerCreatedEvent @event)
    {
        if (!@event.ProductIds.Any() && !@event.Orders.Any())
        {
            return;
        }
        
        foreach (var productId in @event.ProductIds)
        {
            var customerProduct = new CustomerProduct
            {
                CustomerId = @event.CustomerId,
                ProductId = productId
            };

            _dbContext.CustomerProducts.Add(customerProduct);
        }
        
        foreach (var order in @event.Orders)
        {
            var customerOrder = new CustomerOrder
            {
                CustomerId = @event.CustomerId,
                OrderId = order.OrderId,
                CentreId = order.CentreId
            };

            _dbContext.CustomerOrders.Add(customerOrder);
        }

        _changesMade = true;
    }

    private void HandleCustomerProductAddedEvent(CustomerProductAddedEvent @event)
    {
        var customerProduct = new CustomerProduct
        {
            CustomerId = @event.CustomerId,
            ProductId = @event.ProductId
        };

        _dbContext.CustomerProducts.Add(customerProduct);
        _changesMade = true;
    }

    private void HandleCustomerProductRemovedEvent(CustomerProductRemovedEvent @event)
    {
        var customerProduct = new CustomerProduct { CustomerId = @event.CustomerId, ProductId = @event.ProductId };

        _dbContext.CustomerProducts.Remove(customerProduct);
        _changesMade = true;
    }
    
    private void HandleCustomerOrderAddedEvent(CustomerOrderAddedEvent @event)
    {
        var customerOrder = new CustomerOrder
        {
            CustomerId = @event.CustomerId,
            OrderId = @event.Order.OrderId,
            CentreId = @event.Order.CentreId
        };

        _dbContext.CustomerOrders.Add(customerOrder);
        _changesMade = true;
    }

    private void HandleCustomerOrderRemovedEvent(CustomerOrderRemovedEvent @event)
    {
        var customerOrder = new CustomerOrder { CustomerId = @event.CustomerId, OrderId = @event.Order.OrderId };

        _dbContext.CustomerOrders.Remove(customerOrder);
        _changesMade = true;
    }
    
    private void HandleProductCreatedEvent(ProductCreatedEvent @event)
    {
        if (!@event.Orders.Any())
        {
            return;
        }
        
        foreach (var order in @event.Orders)
        {
            var productOrder = new ProductOrder
            {
                ProductId = @event.ProductId,
                OrderId = order.OrderId,
                CentreId = order.CentreId
            };

            _dbContext.ProductOrders.Add(productOrder);
        }

        _changesMade = true;
    }

    private void HandleProductOrderAddedEvent(ProductOrderAddedEvent @event)
    {
        var productOrder = new ProductOrder
        {
            ProductId = @event.ProductId,
            OrderId = @event.Order.OrderId,
            CentreId = @event.Order.CentreId
        };

        _dbContext.ProductOrders.Add(productOrder);
        _changesMade = true;
    }

    private void HandleProductOrderRemovedEvent(ProductOrderRemovedEvent @event)
    {
        var productOrder = new ProductOrder { ProductId = @event.ProductId, OrderId = @event.Order.OrderId };

        _dbContext.ProductOrders.Remove(productOrder);
        _changesMade = true;
    }
}