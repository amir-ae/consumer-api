using Consumer.API.Contract.V1.Customers.Messages;
using Consumer.API.Contract.V1.Products.Messages;

namespace Consumer.Application.Common.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    ICustomerRepository CustomerRepository { get; }
    IProductRepository ProductRepository { get; }
    List<CustomerUpdateMessage> CustomerUpdateMessages { get; }
    List<ProductUpdateMessage> ProductUpdateMessages { get; }
    Task SaveChangesAsync(CancellationToken ct = default);
}