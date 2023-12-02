using Consumer.API.Contract.V1.Customers.Messages;
using Consumer.API.Contract.V1.Products.Messages;
using Consumer.Application.Customers.Commands.AddOrder;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products;

namespace Consumer.Application.Common.Interfaces.Services;

public interface IOrderingService
{
    Task PublishOrderAsync(OrderForCreationMessage orderForCreationMessage, CancellationToken ct = default);
    Task PublishCustomerUpdateAsync(CustomerUpdateMessage customerUpdateMessage, CancellationToken ct = default);
    Task PublishProductUpdateAsync(ProductUpdateMessage productUpdateMessage, CancellationToken ct = default);
}