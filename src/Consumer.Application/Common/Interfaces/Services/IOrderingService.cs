using Consumer.API.Contract.V1.Customers.Messages;
using Consumer.API.Contract.V1.Products.Messages;

namespace Consumer.Application.Common.Interfaces.Services;

public interface IOrderingService
{
    Task PublishCustomerUpdateAsync(CustomerUpdateMessage updateMessage, CancellationToken ct = default);
    Task PublishProductUpdateAsync(ProductUpdateMessage updateMessage, CancellationToken ct = default);
}