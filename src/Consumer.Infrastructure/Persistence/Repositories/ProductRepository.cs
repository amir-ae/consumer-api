using System.Linq.Expressions;
using Marten;
using Marten.Linq;
using Marten.Pagination;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers;
using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;
using Consumer.Infrastructure.Persistence.Extensions;

namespace Consumer.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDocumentSession _session;

    public ProductRepository(IDocumentSession session)
    {
        _session = session;
    }

    public class CheckByIdQuery : ICompiledQuery<Product, bool>
    {
        public string Id { get; init; } = string.Empty;
        
        public Expression<Func<IMartenQueryable<Product>, bool>> QueryIs()
        {
            return q => q.Any(p => p.Id.Value == Id);
        }
    }
    
    public async Task<bool> CheckByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new CheckByIdQuery { Id = id.Value }, ct);
    }
    
    public class ByIdQuery : ICompiledQuery<Product, Product?>
    {
        public string Id { get; init; } = string.Empty;
        
        public Expression<Func<IMartenQueryable<Product>, Product?>> QueryIs()
        {
            return q => q.FirstOrDefault(c => c.Id.Value == Id);
        }
    }
    
    public async Task<Product?> ByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new ByIdQuery { Id = id.Value }, ct);
    }

    public async Task<Product?> DetailByIdAsync(ProductId productId, CancellationToken ct = default)
    {
        Customer? owner = null;
        Customer? dealer = null;
        var product = await _session.Query<Product>()
            .Include<Customer>(x => x.OwnerId!.Value, c => owner = c)
            .Include<Customer>(x => x.DealerId!.Value, c => dealer = c)
            .FirstOrDefaultAsync(p => p.Id.Value == productId.Value, token: ct);

        if (product is null) return null;
        product.Owner = owner;
        product.Dealer = dealer;
        
        return product;
    }

    public async Task<ProductEvents> EventsByIdAsync(ProductId id, CancellationToken ct = default)
    {
        var events = (await _session.Events.FetchStreamAsync(id.Value, token: ct)).ToList();
        var createdEvent = events.First(x => x.EventType == typeof(ProductCreatedEvent)).Data as ProductCreatedEvent;
        return events.Sort(new ProductEvents(createdEvent!));
    }

    public async Task<Product?> DetailByOrderIdAsync(OrderId orderId, CancellationToken ct = default)
    {
        Customer? owner = null;
        Customer? dealer = null;
        var product = await _session.Query<Product>()
            .Include<Customer>(x => x.OwnerId!.Value, c => owner = c)
            .Include<Customer>(x => x.DealerId!.Value, c => dealer = c)
            .FirstOrDefaultAsync(p => p.Orders.Any(order => order.OrderId.Value == orderId.Value), token: ct);
        
        if (product is null) return null;
        product.Owner = owner;
        product.Dealer = dealer;
        
        return product;
    }

    /*public async Task<List<Product>> DetailByOrderIdsAsync(List<OrderId> ids, CancellationToken ct = default)
    {
        var orderIds = ids.Select(id => id.Value);
        var owners = new List<Customer>();
        var dealers = new List<Customer>();
        var products = await _session.Query<Product>()
            .Include<Customer>(x => x.OwnerId!.Value,
            c => owners.Add(c))
            .Include<Customer>(x => x.DealerId!.Value,
                c => dealers.Add(c))
            .Where(p => p.Orders.Any(order => orderIds.Contains(order.OrderId.Value)))
            .ToListAsync(token: ct);

        return AddCustomerDetailToProducts(products, owners, dealers);
    }*/

    public async Task<List<Product>> DetailByCentreIdAsync(CentreId centreId, CancellationToken ct = default)
    {
        var owners = new List<Customer>();
        var dealers = new List<Customer>();
        var products = await _session.Query<Product>()
            .Include<Customer>(x => x.OwnerId!.Value, c => owners.Add(c))
            .Include<Customer>(x => x.DealerId!.Value, c => dealers.Add(c))
            .Where(product => product.Orders.Any(order => order.CentreId.Value == centreId.Value))
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .ToListAsync(token: ct);

        return AddCustomerDetailToProducts(products, owners, dealers);
    }

    public class ByPageQuery : ICompiledListQuery<Product>
    {
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs()
        {
            return q => q.Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
        }
    }
    
    public async Task<(List<Product>, long)> ByPageAsync(int pageSize, int pageIndex, bool? nextPage, ProductId? keyId, 
        CancellationToken ct = default)
    {
        List<Product> products;
        long totalCount;

        if (!nextPage.HasValue || keyId is null || !await CheckByIdAsync(keyId, ct))
        {
            var result = await _session.Query<Product>()
                .Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .ToPagedListAsync(pageIndex, pageSize, token: ct);

            totalCount = result.TotalItemCount;
            products = result.ToList();
        }
        else
        {
            var keyRecord = await ByIdAsync(keyId, ct);
            
            if (nextPage == true)
            {
                var result = (await _session.QueryAsync(new ByPageQuery(), ct)).ToList();

                totalCount = result.Count;
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                var result = (await _session.QueryAsync(new ByPageQuery(), ct)).Reverse().ToList();

                totalCount = result.Count;
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .Reverse()
                    .ToList();
            }
        }
        
        return (products, totalCount);
    }

    public async Task<(List<Product>, long)> ByPageDetailAsync(int pageSize, int pageIndex, bool? nextPage, 
        ProductId? keyId, CancellationToken ct = default)
    {
        List<Product> products;
        List<Customer> owners = new();
        List<Customer> dealers = new();
        long totalCount;

        if (!nextPage.HasValue || keyId is null || !await CheckByIdAsync(keyId, ct))
        {
            var result = await _session.Query<Product>()
                .Include<Customer>(p => p.OwnerId!.Value, c => owners.Add(c))
                .Include<Customer>(p => p.DealerId!.Value, c => dealers.Add(c))
                .Where(p => p.IsDeleted != true)
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.LastModifiedAt)
                .ToPagedListAsync(pageIndex, pageSize, token: ct);

            products = result.ToList();
            totalCount = result.TotalItemCount;
        }
        else
        {
            var keyRecord = await ByIdAsync(keyId, ct);
            var pageTask = _session.Query<Product>()
                .Include<Customer>(p => p.OwnerId!.Value, c => owners.Add(c))
                .Include<Customer>(p => p.DealerId!.Value, c => dealers.Add(c))
                .Where(p => p.IsDeleted != true)
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.LastModifiedAt)
                .ToListAsync(ct);

            if (nextPage == true)
            {
                var result = (await pageTask).ToList();

                totalCount = result.Count;
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                var result = (await pageTask).Reverse().ToList();

                totalCount = result.Count;
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .Reverse()
                    .ToList();
            }
        }

        return (AddCustomerDetailToProducts(products, owners, dealers), totalCount);
    }

    public class AllQuery : ICompiledListQuery<Product>
    {
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs()
        {
            return q => q.Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
        }
    }
    
    public async Task<List<Product>> AllAsync(CancellationToken ct = default)
    {
        var products = await _session.QueryAsync(new AllQuery(), ct);
        return products.ToList();
    }
    
    public class AllCustomersQuery : ICompiledListQuery<Customer>
    {
        public Expression<Func<IMartenQueryable<Customer>, IEnumerable<Customer>>> QueryIs()
        {
            return q => q.Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
        }
    }

    public async Task<List<Product>> AllDetailAsync(CancellationToken ct = default)
    {
        var batch = _session.CreateBatchQuery();
        var productsTask = batch.Query(new AllQuery());
        var customersTask = batch.Query(new AllCustomersQuery());
        await batch.Execute(ct);

        var products = await productsTask;
        var customers = await customersTask;

        return AddCustomerDetailToProducts(products, customers);
    }

    public async Task<Product> CreateAsync(ProductCreatedEvent productCreatedEvent, CancellationToken ct = default)
    {
        var productId = productCreatedEvent.ProductId.Value;
        _session.Events.StartStream<Product>(productId, productCreatedEvent);
        await _session.SaveChangesAsync(ct);
        return (await _session.LoadAsync<Product>(productId, ct))!;
    }

    public async Task<Product> UpdateAsync(ProductEvent productEvent, Product? product = null, CancellationToken ct = default)
    {
        var productId = productEvent.ProductId.Value;
        if (product is null)
        {
            var stream = await _session.Events.FetchForWriting<Product>(productId, ct);
            stream.AppendOne(productEvent);
            await _session.SaveChangesAsync(ct);
            product = stream.Aggregate;
        }
        else
        {
            _session.Events.Append(productId, productEvent);
        }
        return productEvent switch
        {
            ProductBrandChangedEvent e => product.Apply(e),
            ProductModelChangedEvent e => product.Apply(e),
            ProductDealerChangedEvent e => product.Apply(e),
            ProductOrderAddedEvent e => product.Apply(e),
            ProductOrderRemovedEvent e => product.Apply(e),
            ProductDeviceTypeChangedEvent e => product.Apply(e),
            ProductPanelChangedEvent e => product.Apply(e),
            ProductWarrantyCardNumberChangedEvent e => product.Apply(e),
            ProductPurchaseDataChangedEvent e => product.Apply(e),
            ProductUnrepairableEvent e => product.Apply(e),
            _ => product
        };
    }
    
    public async Task<Product> ActivateAsync(ProductActivatedEvent productActivatedEvent, CancellationToken ct = default)
    {
        var productId = productActivatedEvent.ProductId.Value;
        var stream = await _session.Events.FetchForWriting<Product>(productId, ct);
        stream.AppendOne(productActivatedEvent);
        await _session.SaveChangesAsync(ct);
        return stream.Aggregate.Apply(productActivatedEvent);
    }

    public async Task<Product> DeactivateAsync(ProductDeactivatedEvent productDeactivatedEvent, CancellationToken ct = default)
    {
        var productId = productDeactivatedEvent.ProductId.Value;
        var stream = await _session.Events.FetchForWriting<Product>(productId, ct);
        stream.AppendOne(productDeactivatedEvent);
        await _session.SaveChangesAsync(ct);
        return stream.Aggregate.Apply(productDeactivatedEvent);
    }

    public async Task<Product> DeleteAsync(ProductDeletedEvent productDeletedEvent, CancellationToken ct = default)
    {
        var productId = productDeletedEvent.ProductId.Value;
        var stream = await _session.Events.FetchForWriting<Product>(productId, ct);
        stream.AppendOne(productDeletedEvent);
        await _session.SaveChangesAsync(ct);
        return stream.Aggregate.Apply(productDeletedEvent);
    }

    public async Task<Product> UndeleteAsync(ProductUndeletedEvent productUndeletedEvent, CancellationToken ct = default)
    {
        var productId = productUndeletedEvent.ProductId.Value;
        var stream = await _session.Events.FetchForWriting<Product>(productId, ct);
        stream.AppendOne(productUndeletedEvent);
        await _session.SaveChangesAsync(ct);
        return stream.Aggregate.Apply(productUndeletedEvent);
    }
    
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _session.SaveChangesAsync(ct);
    }
    
    private List<Product> AddCustomerDetailToProducts(IEnumerable<Product> products, IEnumerable<Customer> customers, 
        IList<Customer>? dealers = null)
    {
        foreach (var product in products)
        {
            product.Owner = customers.FirstOrDefault(c => c.Id == product.OwnerId);
            product.Dealer = (dealers ?? customers).FirstOrDefault(c => c.Id == product.DealerId);
        }

        return products.ToList();
    }
}
