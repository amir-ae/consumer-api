using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;

namespace Consumer.Infrastructure.Services;

public class EventSubscriptionManager : IEventSubscriptionManager
{
    private readonly List<(Customer Customer, Action<CustomerEvent> Handler)> _customerSubscriptions = new();
    private readonly List<(Product Product, Action<ProductEvent> Handler)> _productSubscriptions = new();
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public EventSubscriptionManager(ICustomerRepository customerRepository, IProductRepository productRepository)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
    }

    public void SubscribeToCustomerEvents(Customer customer)
    {
        if (_customerSubscriptions.Any(s => s.Customer == customer)) return;
        void Handler(CustomerEvent @event) => _customerRepository.Append(@event);
        customer.CustomerEventHandler.EventOccurred += Handler;
        _customerSubscriptions.Add((customer, Handler));
    }

    public void UnsubscribeFromCustomerEvents(Customer customer)
    {
        var subscription = _customerSubscriptions.FirstOrDefault(s => s.Customer == customer);
        if (subscription != default)
        {
            customer.CustomerEventHandler.EventOccurred -= subscription.Handler;
            _customerSubscriptions.Remove(subscription);
        }
    }
    
    public void SubscribeToProductEvents(Product product)
    {
        if (_productSubscriptions.Any(s => s.Product == product)) return;
        void Handler(ProductEvent @event) => _productRepository.Append(@event);
        product.ProductEventHandler.EventOccurred += Handler;
        _productSubscriptions.Add((product, Handler));
    }

    public void UnsubscribeFromProductEvents(Product product)
    {
        var subscription = _productSubscriptions.FirstOrDefault(s => s.Product == product);
        if (subscription != default)
        {
            product.ProductEventHandler.EventOccurred -= subscription.Handler;
            _productSubscriptions.Remove(subscription);
        }
    }

    public void Dispose()
    {
        foreach (var (customer, handler) in _customerSubscriptions)
        {
            customer.CustomerEventHandler.EventOccurred -= handler;
        }
        _customerSubscriptions.Clear();
        
        foreach (var (product, handler) in _productSubscriptions)
        {
            product.ProductEventHandler.EventOccurred -= handler;
        }
        _productSubscriptions.Clear();
    }
}