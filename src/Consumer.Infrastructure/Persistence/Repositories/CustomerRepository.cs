using System.Linq.Expressions;
using Marten;
using Marten.Linq;
using Marten.Pagination;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Infrastructure.Persistence.Extensions;

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
        
        public Expression<Func<IMartenQueryable<Customer>, bool>> QueryIs()
        {
            return q => q.Any(c => c.Id.Value == Id);
        }
    }

    public async Task<bool> CheckByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new CheckByIdQuery { Id = id.Value }, ct);
    }

    public class ByIdQuery : ICompiledQuery<Customer, Customer?>
    {
        public string Id { get; init; } = string.Empty;
        
        public Expression<Func<IMartenQueryable<Customer>, Customer?>> QueryIs()
        {
            return q => q.FirstOrDefault(c => c.Id.Value == Id);
        }
    }
    
    public async Task<Customer?> ByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        return await _session.QueryAsync(new ByIdQuery { Id = id.Value }, ct);
    }

    public class ByDataQuery : ICompiledQuery<Customer, Customer?>
    {
        public string FullName { get; init; } = string.Empty;
        public string PhoneNumber { get; init; } = string.Empty;
        
        public Expression<Func<IMartenQueryable<Customer>, Customer?>> QueryIs()
        {
            return q => q
                .FirstOrDefault(c => c.FullName == FullName && c.PhoneNumber == PhoneNumber);
        }
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
        customer.Products = products.ToHashSet();
        return customer;
    }

    public async Task<List<Customer>> DetailByIdsAsync(List<CustomerId> ids, CancellationToken ct = default)
    {
        var customerIds = ids.Select(id => id.Value);
        var customers = (await _session.LoadManyAsync<Customer>(ct, customerIds)).ToList();
        var productIds = customers.SelectMany(c => c.ProductIds).Select(id => id.Value);
        var products = (await _session.LoadManyAsync<Product>(ct, productIds)).ToList();
        return await AddProductDetailToCustomers(customers, products);
    }
    
    public async Task<CustomerEvents> EventsByIdAsync(CustomerId id, CancellationToken ct = default)
    {
        var events = (await _session.Events.FetchStreamAsync(id.Value, token: ct)).ToList();
        var createdEvent = events.First(x => x.EventType == typeof(CustomerCreatedEvent)).Data as CustomerCreatedEvent;
        return events.Sort(new CustomerEvents(createdEvent!));
    }
    
    public class ByPageQuery : ICompiledListQuery<Customer>
    {
        public Expression<Func<IMartenQueryable<Customer>, IEnumerable<Customer>>> QueryIs()
        {
            return q => q.Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
        }
    }

    public async Task<(List<Customer>, long)> ByPageAsync(int pageSize, int pageIndex, bool? nextPage, CustomerId? keyId, 
        CancellationToken ct = default)
    {
        List<Customer> customers;
        long totalCount;

        if (!nextPage.HasValue || keyId is null || !await CheckByIdAsync(keyId, ct))
        {
            var result = await _session.Query<Customer>()
                .Where(c => c.IsDeleted != true)
                .OrderByDescending(c => c.CreatedAt)
                .ThenByDescending(c => c.LastModifiedAt)
                .ToPagedListAsync(pageIndex, pageSize, token: ct);

            totalCount = result.TotalItemCount;
            customers = result.ToList();
        }
        else
        {
            var keyRecord = await ByIdAsync(keyId, ct);
            
            if (nextPage == true)
            {
                var result = (await _session.QueryAsync(new ByPageQuery(), ct)).ToList();

                totalCount = result.Count;
                customers = result
                    .SkipWhile(c => c.Id != keyRecord!.Id)
                    .Where(c => c.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .ToList();
            }
            else
            {
                var result = (await _session.QueryAsync(new ByPageQuery(), ct)).Reverse().ToList();

                totalCount = result.Count;
                customers = result
                    .SkipWhile(c => c.Id != keyRecord!.Id)
                    .Where(c => c.Id != keyRecord!.Id)
                    .Take(pageSize)
                    .Reverse()
                    .ToList();
            }
        }
        
        return (customers, totalCount);
    }

    public async Task<(List<Customer>, long)> ByPageDetailAsync(int pageSize, int pageIndex, bool? nextPage, 
        CustomerId? keyId, CancellationToken ct = default)
    {
        var (customers, totalCount) = await ByPageAsync(pageSize, pageIndex, nextPage, keyId, ct);

        var productIds = customers.SelectMany(c => c.ProductIds).Select(id => id.Value);
        var products = (await _session.LoadManyAsync<Product>(ct, productIds)).ToList();

        return (await AddProductDetailToCustomers(customers, products), totalCount);
    }

    public class AllQuery : ICompiledListQuery<Customer>
    {
        public Expression<Func<IMartenQueryable<Customer>, IEnumerable<Customer>>> QueryIs()
        {
            return q => q.Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
        }
    }
    
    public async Task<List<Customer>> AllAsync(CancellationToken ct = default)
    {
        var customers = await _session.QueryAsync(new AllQuery(), ct);
        return customers.ToList();
    }
    
    public class AllProductsQuery : ICompiledListQuery<Product>
    {
        public Expression<Func<IMartenQueryable<Product>, IEnumerable<Product>>> QueryIs()
        {
            return q => q.Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt);
        }
    }
    
    public async Task<List<Customer>> AllDetailAsync(CancellationToken ct = default)
    {
        var batch = _session.CreateBatchQuery();
        var customersTask = batch.Query(new AllQuery());
        var productsTask = batch.Query(new AllProductsQuery());
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

    public async Task<Customer> UpdateAsync(CustomerEvent customerEvent, Customer? customer = null, CancellationToken ct = default)
    {
        var customerId = customerEvent.CustomerId.Value;
        if (customer is null)
        {
            var stream = await _session.Events.FetchForWriting<Customer>(customerId, ct);
            stream.AppendOne(customerEvent);
            await _session.SaveChangesAsync(ct);
            customer = stream.Aggregate;
        }
        else
        {
            _session.Events.Append(customerId, customerEvent);
        }

        return customerEvent switch
        {
            CustomerNameChangedEvent e => customer.Apply(e),
            CustomerPhoneNumberChangedEvent e => customer.Apply(e),
            CustomerAddressChangedEvent e => customer.Apply(e),
            CustomerRoleChangedEvent e => customer.Apply(e),
            CustomerProductAddedEvent e => customer.Apply(e),
            CustomerProductRemovedEvent e => customer.Apply(e),
            _ => customer
        };
    }
    
    public async Task<Customer> ActivateAsync(CustomerActivatedEvent customerActivatedEvent, CancellationToken ct = default)
    {
        var customerId = customerActivatedEvent.CustomerId.Value;
        var stream = await _session.Events.FetchForWriting<Customer>(customerId, ct);
        stream.AppendOne(customerActivatedEvent);
        await _session.SaveChangesAsync(ct);
        return stream.Aggregate.Apply(customerActivatedEvent);
    }

    public async Task<Customer> DeactivateAsync(CustomerDeactivatedEvent customerDeactivatedEvent, CancellationToken ct = default)
    {
        var customerId = customerDeactivatedEvent.CustomerId.Value;
        var stream = await _session.Events.FetchForWriting<Customer>(customerId, ct);
        stream.AppendOne(customerDeactivatedEvent);
        await _session.SaveChangesAsync(ct);
        return stream.Aggregate.Apply(customerDeactivatedEvent);
    }

    public async Task<Customer> DeleteAsync(CustomerDeletedEvent customerDeletedEvent, CancellationToken ct = default)
    {
        var customerId = customerDeletedEvent.CustomerId.Value;
        var stream = await _session.Events.FetchForWriting<Customer>(customerId, ct);
        stream.AppendOne(customerDeletedEvent);
        await _session.SaveChangesAsync(ct);
        return stream.Aggregate.Apply(customerDeletedEvent);
    }

    public async Task<Customer> UndeleteAsync(CustomerUndeletedEvent customerUndeletedEvent, CancellationToken ct = default)
    {
        var customerId = customerUndeletedEvent.CustomerId.Value;
        var stream = await _session.Events.FetchForWriting<Customer>(customerId, ct);
        stream.AppendOne(customerUndeletedEvent);
        await _session.SaveChangesAsync(ct);
        return stream.Aggregate.Apply(customerUndeletedEvent);
    }
    
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _session.SaveChangesAsync(ct);
    }
    
    private async Task<List<Customer>> AddProductDetailToCustomers(IEnumerable<Customer> customers, IEnumerable<Product> products)
    {
        foreach (var customer in customers)
        {
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
                    await UpdateAsync(customerProductAddedEvent);
                }
                var productIdsToRemove = customer.ProductIds.Except(customerProductIds);
                foreach (var productId in productIdsToRemove)
                {
                    var customerProductRemovedEvent = new CustomerProductRemovedEvent(
                        customer.Id,
                        productId,
                        new AppUserId(Guid.Empty),
                        DateTimeOffset.UtcNow);
                    await UpdateAsync(customerProductRemovedEvent);
                }
                customer.ProductIds = customerProducts.Select(p => p.Id).ToHashSet();
            }
            customer.Products = customerProducts;
        }
        return customers.ToList();
    }
}
