namespace Commerce.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    ICustomerRepository CustomerRepository { get; }
    IProductRepository ProductRepository { get; }
    void Store<TEntity>(string id, TEntity? entity);
    Task SaveChangesAsync(CancellationToken ct = default);
}