using System.Linq.Expressions;
using Marten;
using Marten.Linq;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;
using Consumer.Infrastructure.Persistence.Extensions;
using Marten.Events.CodeGeneration;

namespace Consumer.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDocumentSession _session;

    public CustomerRepository(IDocumentSession session)
    {
        _session = session;
    }

    public class CheckByIdQuery : ICompiledQuery<Customer, bool>
    {
        public string Id { get; init; } = string.Empty;
        public Expression<Func<IMartenQueryable<Customer>, bool>> QueryIs() => query 
            => query.Any(c => c.Id.Value == Id);
    }

    public async Task<bool> CheckByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new CheckByIdQuery { Id = id.Value }, ct);
    }

    public class ByIdQuery : ICompiledQuery<Customer, Customer?>
    {
        public string Id { get; init; } = string.Empty;
        public Expression<Func<IMartenQueryable<Customer>, Customer?>> QueryIs() => query 
            => query.FirstOrDefault(c => c.Id.Value == Id);
    }
    
    public async Task<Customer?> ByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new ByIdQuery { Id = id.Value }, ct);
    }
    
    public async Task<Customer?> ByStreamIdAsync(CustomerId id, CancellationToken ct = default)
    {
        return await _session.Events.AggregateStreamAsync<Customer>(id.Value, token: ct);
    }

    public class ByDataQuery : ICompiledQuery<Customer, Customer?>
    {
        public string FullName { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        public Expression<Func<IMartenQueryable<Customer>, Customer?>> QueryIs() => query 
            => query.FirstOrDefault(c => c.FullName == FullName && c.PhoneNumber == PhoneNumber);
    }
    
    public async Task<Customer?> ByDataAsync(string fullName, string phoneNumber, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new ByDataQuery { FullName = fullName, PhoneNumber = phoneNumber }, ct);
    }
    
    public async Task<Customer?> DetailByIdAsync(CustomerId customerId, CancellationToken ct = default)
    {
        var customer = await _session.QueryAsync(new ByIdQuery { Id = customerId.Value }, ct);
        if (customer is null || !customer.ProductIds.Any()) return customer;
        var productIds = customer.ProductIds.Select(id => id.Value);
        var products = await _session.LoadManyAsync<Product>(ct, productIds);
        return customer.AddProducts(products.ToHashSet());
    }

    public async Task<List<Customer>> DetailByIdsAsync(List<CustomerId> ids, CancellationToken ct = default)
    {
        var customerIds = ids.Select(id => id.Value);
        var customers = (await _session.LoadManyAsync<Customer>(ct, customerIds)).ToList();
        var productIds = customers.SelectMany(c => c.ProductIds).Select(id => id.Value);
        var products = (await _session.LoadManyAsync<Product>(ct, productIds)).ToList();
        return await AddProductDetailToCustomers(customers, products);
    }
    
    public async Task<CustomerEvents?> EventsByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        var events = (await _session.Events.FetchStreamAsync(id.Value, token: ct)).ToList();
        var createdEvent = events.FirstOrDefault(x => x.EventType == typeof(CustomerCreatedEvent))?.Data as CustomerCreatedEvent;
        if (createdEvent is null) return null;
        return events.Sort(new CustomerEvents(createdEvent));
    }
    
    public class ByPageQuery : ICompiledListQuery<Customer>
    {
        [MartenIgnore]
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int SkipSize => PageSize * (PageNumber - 1);
        public Expression<Func<IMartenQueryable<Customer>, IEnumerable<Customer>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .Skip(SkipSize)
                .Take(PageSize);
        public QueryStatistics Statistics { get; } = new();
    }
    
    public class ByCentrePageQuery : ICompiledListQuery<Customer>
    {
        [MartenIgnore]
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public Guid CentreId { get; init; }
        public int SkipSize => PageSize * (PageNumber - 1);
        public Expression<Func<IMartenQueryable<Customer>, IEnumerable<Customer>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted && x.Orders.Any(o => o.CentreId.Value == CentreId))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .Skip(SkipSize)
                .Take(PageSize);
        public QueryStatistics Statistics { get; } = new();
    }
    
    public class NextPageQuery : ICompiledListQuery<Customer>
    {
        public Expression<Func<IMartenQueryable<Customer>, IEnumerable<Customer>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public class NextCentrePageQuery : ICompiledListQuery<Customer>
    {
        public Guid CentreId { get; init; }
        public Expression<Func<IMartenQueryable<Customer>, IEnumerable<Customer>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted && x.Orders.Any(o => o.CentreId.Value == CentreId))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }

    public async Task<(List<Customer>, long)> ByPageAsync(int pageSize, int pageNumber, bool? nextPage, CustomerId? keyId, 
        CentreId? centreId, CancellationToken ct = default)
    {
        List<Customer> customers;
        long totalCount;

        if (!nextPage.HasValue || keyId is null || !await CheckByIdAsync(keyId, ct))
        {
            if (centreId is null)
            {
                var query = new ByPageQuery { PageNumber = pageNumber, PageSize = pageSize };
                customers = (await _session.QueryAsync(query, ct)).ToList();
                totalCount = query.Statistics.TotalResults;
            }
            else
            {
                var query = new ByCentrePageQuery { PageNumber = pageNumber, PageSize = pageSize, CentreId = centreId.Value };
                customers = (await _session.QueryAsync(query, ct)).ToList();
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
                customers = result
                    .SkipWhile(c => c.Id != keyRecord!.Id)
                    .Where(c => c.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                result.Reverse();
                customers = result
                    .SkipWhile(c => c.Id != keyRecord!.Id)
                    .Where(c => c.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .Reverse()
                    .ToList();
            }
            
            totalCount = result.Count;
        }
        
        return (customers, totalCount);
    }

    public async Task<(List<Customer>, long)> ByPageDetailAsync(int pageSize, int pageNumber, bool? nextPage, 
        CustomerId? keyId, CentreId? centreId, CancellationToken ct = default)
    {
        var (customers, totalCount) = await ByPageAsync(pageSize, pageNumber, nextPage, keyId, centreId, ct);

        var productIds = customers.SelectMany(c => c.ProductIds).Select(id => id.Value);
        var products = (await _session.LoadManyAsync<Product>(ct, productIds)).ToList();

        return (await AddProductDetailToCustomers(customers, products), totalCount);
    }

    public class ListQuery : ICompiledListQuery<Customer>
    {
        public Expression<Func<IMartenQueryable<Customer>, IEnumerable<Customer>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public class ByCentreIdQuery : ICompiledListQuery<Customer>
    {
        public Guid CentreId { get; init; }
        public Expression<Func<IMartenQueryable<Customer>, IEnumerable<Customer>>> QueryIs() => query
             => query.Where(x => !x.IsDeleted 
                                         && x.Orders.Any(o => o.CentreId.Value == CentreId))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public async Task<List<Customer>> ListAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var customers = centreId is null
            ? await _session.QueryAsync(new ListQuery(), ct)
            : await _session.QueryAsync(new ByCentreIdQuery { CentreId = centreId.Value }, ct);
        
        return customers.ToList();
    }
    
    public class ListProductsQuery : ICompiledListQuery<Product>
    {
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query 
            => query.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }
    
    public class ByCentreIdProductsQuery : ICompiledListQuery<Product>
    {
        public Guid CentreId { get; init; }
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs() => query
             => query.Where(x => !x.IsDeleted 
                                         && x.Orders.Any(o => o.CentreId.Value == CentreId))
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
    }

    public async Task<List<Customer>> ListDetailAsync(CentreId? centreId = null, CancellationToken ct = default)
    {
        var batch = _session.CreateBatchQuery();
        var customersTask = centreId is null ? batch.Query(new ListQuery()) 
            : batch.Query(new ByCentreIdQuery { CentreId = centreId.Value });
        var productsTask = centreId is null ? batch.Query(new ListProductsQuery())
            : batch.Query(new ByCentreIdProductsQuery { CentreId = centreId.Value });
        await batch.Execute(ct);

        var customers = await customersTask;
        var products = await productsTask;

        return await AddProductDetailToCustomers(customers, products);
    }

    public async Task<Customer> CreateAsync(CustomerCreatedEvent customerCreatedEvent, CancellationToken ct = default)
    {
        var customerId = customerCreatedEvent.CustomerId.Value;
        _session.Events.StartStream<Customer>(customerId, customerCreatedEvent);
        await _session.SaveChangesAsync(ct);
        return (await _session.LoadAsync<Customer>(customerId, ct))!;
    }

    public void Append(CustomerEvent customerEvent)
    {
        var customerId = customerEvent.CustomerId.Value;
        _session.Events.Append(customerId, customerEvent);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _session.SaveChangesAsync(ct);
    }
    
    private async Task<List<Customer>> AddProductDetailToCustomers(
        IEnumerable<Customer> customersEnumerable, 
        IEnumerable<Product> productsEnumerable)
    {
        var customers = customersEnumerable as Customer[] ?? customersEnumerable.ToArray();
        var products = productsEnumerable as Product[] ?? productsEnumerable.ToArray();
        
        for (var i = 0; i < customers.Length; i++)
        {
            var customer = customers[i];
            var customerProducts = products.Where(p => 
                customer.Id.Value == p.OwnerId?.Value || customer.Id.Value == p.DealerId?.Value).ToHashSet();
            if (!customer.ProductIds.SetEquals(customerProducts.Select(p => p.Id).ToHashSet()))
            {
                var customerProductIds = customerProducts.Select(p => p.Id).ToHashSet();
                var productIdsToAdd = customerProductIds.Except(customer.ProductIds);
                foreach (var productId in productIdsToAdd)
                {
                    var customerProductAddedEvent = new CustomerProductAddedEvent(
                        customer.Id,
                        productId,
                        new AppUserId(Guid.Empty),
                        DateTimeOffset.UtcNow);
                    Append(customerProductAddedEvent);
                }
                var productIdsToRemove = customer.ProductIds.Except(customerProductIds);
                foreach (var productId in productIdsToRemove)
                {
                    var product = await _session.LoadAsync<Product>(productId.Value);
                    if (product is null || customer.Id != product.OwnerId && customer.Id != product.DealerId)
                    {
                        var customerProductRemovedEvent = new CustomerProductRemovedEvent(
                            customer.Id,
                            productId,
                            new AppUserId(Guid.Empty),
                            DateTimeOffset.UtcNow);
                        Append(customerProductRemovedEvent);
                    }
                    else
                    {
                        customerProducts.Add(product);
                    }
                }
                await SaveChangesAsync();
                customer = customer.SetProductIds(customerProducts.Select(p => p.Id).ToHashSet());
            }
            customers[i] = customer.AddProducts(customerProducts);
        }
        return customers.ToList();
    }
}
