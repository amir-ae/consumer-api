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

    public async Task PublishCustomerUpdateAsync(CustomerUpdateMessage updateMessage, CancellationToken ct = default)
    {
        await _bus.Publish(updateMessage, ct);
    }
    
    public async Task PublishProductUpdateAsync(ProductUpdateMessage updateMessage, CancellationToken ct = default)
    {
        await _bus.Publish(updateMessage, ct);
    }
}