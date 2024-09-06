using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.Infrastructure.Customers.Repositories;
using Commerce.Infrastructure.Products.Repositories;
using Marten;

namespace Commerce.Infrastructure.Common.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDocumentSession _session;
    private readonly ICachingService _cachingService;

    public ICustomerRepository CustomerRepository { get; }
    public IProductRepository ProductRepository { get; }

    public UnitOfWork(IDocumentSession session, ICachingService cachingService)
    {
        _session = session;
        CustomerRepository = new CustomerMartenRepository(_session);
        ProductRepository = new ProductMartenRepository(_session);
        _cachingService = cachingService;
    }

    public void Store<TEntity>(string id, TEntity? entity)
        => _cachingService.Store(id, entity);
    
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _session.SaveChangesAsync(ct);

        _cachingService.ClearEntityCache();
    }

    public void Dispose()
    {
        try
        {
            _cachingService.Dispose();
            CustomerRepository.Dispose();
            ProductRepository.Dispose();
            _session.Dispose();
        }
        catch
        {
            //ignore
        }
    }
}