using Consumer.API.Contract.V1.Customers.Messages;
using Consumer.API.Contract.V1.Products.Messages;
using Consumer.Application.Common.Interfaces.Services;
using MassTransit;

namespace Consumer.Infrastructure.Services;

public class OrderingService : IOrderingService
{
    private readonly IBus _bus;

    public OrderingService(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishCustomerUpdateAsync(CustomerUpdateMessage customerUpdateMessage, CancellationToken ct = default)
    {
        await _bus.Publish(customerUpdateMessage, ct);
    }
    
    public async Task PublishProductUpdateAsync(ProductUpdateMessage productUpdateMessage, CancellationToken ct = default)
    {
        await _bus.Publish(productUpdateMessage, ct);
    }
}