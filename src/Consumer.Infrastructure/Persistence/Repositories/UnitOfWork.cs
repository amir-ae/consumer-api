using Consumer.API.Contract.V1.Customers.Messages;
using Consumer.API.Contract.V1.Products.Messages;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Infrastructure.Services;
using Marten;

namespace Consumer.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDocumentSession _session;
    private readonly OrderingService _orderingService;
    
    public ICustomerRepository CustomerRepository { get; }
    public IProductRepository ProductRepository { get; }
    public List<CustomerUpdateMessage> CustomerUpdateMessages { get; } = new();
    public List<ProductUpdateMessage> ProductUpdateMessages { get; } = new();
    
    public UnitOfWork(IDocumentSession session, OrderingService orderingService)
    {
        _session = session;
        CustomerRepository = new CustomerRepository(_session);
        ProductRepository = new ProductRepository(_session);
        _orderingService = orderingService;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _session.SaveChangesAsync(ct);

        await PublishCustomerUpdateMessages(ct);
        
        await PublishProductUpdateMessages(ct);

    }

    private async Task PublishCustomerUpdateMessages(CancellationToken ct = default)
    {
        List<Task> publishTasks = CustomerUpdateMessages
            .Select(message => _orderingService.PublishCustomerUpdateAsync(message, ct)).ToList();
        
        await Task.WhenAll(publishTasks);

        CustomerUpdateMessages.Clear();
    }
    
    private async Task PublishProductUpdateMessages(CancellationToken ct = default)
    {
        List<Task> publishTasks = ProductUpdateMessages
            .Select(message => _orderingService.PublishProductUpdateAsync(message, ct)).ToList();
        
        await Task.WhenAll(publishTasks);

        ProductUpdateMessages.Clear();
    }
    
    public void Dispose()
    {
    }
}