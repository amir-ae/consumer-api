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
using Marten.Events.CodeGeneration;

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
        
        public Expression<Func<IMartenQueryable<Product>, bool>> QueryIs() => query 
            => query.Any(p => p.Id.Value == Id);
    }
    
    public async Task<bool> CheckByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new CheckByIdQuery { Id = id.Value }, ct);
    }
    
    public class ByIdQuery : ICompiledQuery<Product, Product?>
    {
        public string Id { get; init; } = string.Empty;
        public Expression<Func<IMartenQueryable<Product>, Product?>> QueryIs() => query 
            => query.FirstOrDefault(c => c.Id.Value == Id);
    }
    
    public async Task<Product?> ByIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new ByIdQuery { Id = id.Value }, ct);
    }
    
    public async Task<Product?> ByStreamIdAsync(ProductId id, CancellationToken ct = default)
    {
        return await _session.Events.AggregateStreamAsync<Product>(id.Value, token: ct);
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
        product = product.AddOwner(owner);
        product = product.AddDealer(dealer);
        
        return product;
    }

    public async Task<ProductEvents?> EventsByIdAsync(ProductId id, CancellationToken ct = default)
    {
        var events = (await _session.Events.FetchStreamAsync(id.Value, token: ct)).ToList();
        var createdEvent = events.FirstOrDefault(x => x.EventType == typeof(ProductCreatedEvent))?.Data as ProductCreatedEvent;
        if (createdEvent is null) return null;
        return events.Sort(new ProductEvents(createdEvent));
    }

    public async Task<Product?> DetailByOrderIdAsync(OrderId orderId, CancellationToken ct = default)
    {
        Customer? owner = null;
        Customer? dealer = null;
        var product = await _session.Query<Product>()
            .Include<Customer>(x => x.OwnerId!.Value, c => owner = c)
            .Include<Customer>(x => x.DealerId!.Value, c => dealer = c)
            .FirstOrDefaultAsync(p => p.Orders.Any(order => order.Id.Value == orderId.Value), token: ct);
        
        if (product is null) return null;
        product = product.AddOwner(owner);
        product = product.AddDealer(dealer);
        
        return product;
    }
    
    public async Task<List<Product>> DetailByCentreIdAsync(CentreId centreId, CancellationToken ct = default)
    {
        var owners = new List<Customer>();
        var dealers = new List<Customer>();
        var products = await _session.Query<Product>()
            .Include<Customer>(p => p.OwnerId!.Value, c => owners.Add(c))
            .Include<Customer>(p => p.DealerId!.Value, c => dealers.Add(c))
            .Where(p => p.Orders.Any(o => o.CentreId.Value == centreId.Value))
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.LastModifiedAt)
            .ToListAsync(token: ct);

        return AddCustomerDetailToProducts(products, owners, dealers);
    }
    
    public class ByPageQuery : ICompiledListQuery<Product>
    {
        [MartenIgnore]
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int SkipSize => PageSize * (PageNumber - 1);
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .Skip(SkipSize)
                .Take(PageSize);
        public QueryStatistics Statistics { get; } = new();
    }
    
    public class ByCentrePageQuery : ICompiledListQuery<Product>
    {
        [MartenIgnore]
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public Guid CentreId { get; init; }
        public int SkipSize => PageSize * (PageNumber - 1);
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted && x.Orders.Any(o => o.CentreId.Value == CentreId))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .Skip(SkipSize)
                .Take(PageSize);
        public QueryStatistics Statistics { get; } = new();
    }

    public class NextPageQuery : ICompiledListQuery<Product>
    {
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public class NextCentrePageQuery : ICompiledListQuery<Product>
    {
        public Guid CentreId { get; init; }
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted && x.Orders.Any(o => o.CentreId.Value == CentreId))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public async Task<(List<Product>, long)> ByPageAsync(int pageSize, int pageNumber, bool? nextPage, ProductId? keyId, 
        CentreId? centreId, CancellationToken ct = default)
    {
        List<Product> products;
        long totalCount;

        if (!nextPage.HasValue || keyId is null || !await CheckByIdAsync(keyId, ct))
        {
            if (centreId is null)
            {
                var query = new ByPageQuery { PageNumber = pageNumber, PageSize = pageSize };
                products = (await _session.QueryAsync(query, ct)).ToList();
                totalCount = query.Statistics.TotalResults;
            }
            else
            {
                var query = new ByCentrePageQuery { PageNumber = pageNumber, PageSize = pageSize, CentreId = centreId.Value };
                products = (await _session.QueryAsync(query, ct)).ToList();
                totalCount = query.Statistics.TotalResults;
            }
        }
        else
        {
            var keyRecord = await ByIdAsync(keyId, ct);
            var result = centreId is null
                ? (await _session.QueryAsync(new NextPageQuery(), ct)).ToList()
                : (await _session.QueryAsync(new NextCentrePageQuery { CentreId = centreId.Value }, ct)).ToList();
            
            if (nextPage == true)
            {
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                result.Reverse();
                products = result
                    .SkipWhile(p => p.Id != keyRecord!.Id)
                    .Where(p => p.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .Reverse()
                    .ToList();
            }
            
            totalCount = result.Count;
        }
        
        return (products, totalCount);
    }

    public async Task<(List<Product>, long)> ByPageDetailAsync(int pageSize, int pageNumber, bool? nextPage, 
        ProductId? keyId, CentreId? centreId, CancellationToken ct = default)
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
                .Where(centreId is null
                    ? p => !p.IsDeleted
                    : p => !p.IsDeleted && p.Orders.Any(o => o.CentreId.Value == centreId.Value))
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.LastModifiedAt)
                .ToPagedListAsync(pageNumber, pageSize, token: ct);

            products = result.ToList();
            totalCount = result.TotalItemCount;
        }
        else
        {
            var keyRecord = await ByIdAsync(keyId, ct);
            var pageTask = _session.Query<Product>()
                .Include<Customer>(p => p.OwnerId!.Value, c => owners.Add(c))
                .Include<Customer>(p => p.DealerId!.Value, c => dealers.Add(c))
                .Where(centreId is null
                    ? p => !p.IsDeleted
                    : p => !p.IsDeleted && p.Orders.Any(o => o.CentreId.Value == centreId.Value))
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

    public class ListQuery : ICompiledListQuery<Product>
    {
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public class ByCentreIdQuery : ICompiledListQuery<Product>
    {
        public Guid CentreId { get; init; }
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted && x.Orders.Any(o => o.CentreId.Value == CentreId))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public async Task<List<Product>> ListAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var products = centreId is null
            ? await _session.QueryAsync(new ListQuery(), ct)
            : await _session.QueryAsync(new ByCentreIdQuery { CentreId = centreId.Value }, ct);
        
        return products.ToList();
    }

    public async Task<List<Product>> ListDetailAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var owners = new List<Customer>();
        var dealers = new List<Customer>();
        var products = await _session.Query<Product>()
            .Include<Customer>(p => p.OwnerId!.Value, c => owners.Add(c))
            .Include<Customer>(p => p.DealerId!.Value, c => dealers.Add(c))
            .Where(centreId is null
                ? p => !p.IsDeleted
                : p => !p.IsDeleted && p.Orders.Any(o => o.CentreId.Value == centreId.Value))
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.LastModifiedAt)
            .ToListAsync(token: ct);

        return AddCustomerDetailToProducts(products, owners, dealers);
    }

    public async Task<Product> CreateAsync(ProductCreatedEvent productCreatedEvent, CancellationToken ct = default)
    {
        var productId = productCreatedEvent.ProductId.Value;
        _session.Events.StartStream<Product>(productId, productCreatedEvent);
        await _session.SaveChangesAsync(ct);
        return (await _session.LoadAsync<Product>(productId, ct))!;
    }

    public void Append(ProductEvent productEvent)
    {
        var productId = productEvent.ProductId.Value;
        _session.Events.Append(productId, productEvent);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _session.SaveChangesAsync(ct);
    }
    
    private List<Product> AddCustomerDetailToProducts(
        IEnumerable<Product> productsEnumerable, 
        IEnumerable<Customer> customersEnumerable, 
        IEnumerable<Customer>? dealers = null)
    {
        var products = productsEnumerable as Product[] ?? productsEnumerable.ToArray();
        var customers = customersEnumerable as Customer[] ?? customersEnumerable.ToArray();
        
        for (var i = 0; i < products.Length; i++)
        {
            products[i] = products[i].AddOwner(customers.FirstOrDefault(c => c.Id == products[i].OwnerId));
            products[i] = products[i].AddDealer((dealers ?? customers).FirstOrDefault(c => c.Id == products[i].DealerId));
        }

        return products.ToList();
    }
}
