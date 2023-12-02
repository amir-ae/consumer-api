using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Common.Interfaces.Persistence;

public interface IValidatorChecks
{
    Task<bool> CustomerExists(CustomerId customerId, CancellationToken ct = default);
    Task<bool> ProductExists(ProductId productId, CancellationToken ct = default);
}