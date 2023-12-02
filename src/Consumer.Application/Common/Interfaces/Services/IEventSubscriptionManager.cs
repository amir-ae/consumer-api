using Consumer.Domain.Customers;
using Consumer.Domain.Products;

namespace Consumer.Application.Common.Interfaces.Services;

public interface IEventSubscriptionManager : IDisposable
{
    void SubscribeToCustomerEvents(Customer customer);
    void UnsubscribeFromCustomerEvents(Customer customer);
    void SubscribeToProductEvents(Product product);
    void UnsubscribeFromProductEvents(Product product);
}