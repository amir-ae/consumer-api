using Mapster;
using Consumer.API.Contract.V1.Customers.Requests;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.API.Contract.V1.Customers.Responses.Events;
using Consumer.Application.Common.Commands;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers;
using Consumer.Domain.Common.Entities;
using OrderResponse = Consumer.API.Contract.V1.Common.Order;

namespace Consumer.API.Extensions.Mapping;

public class CustomerMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, UpsertProductCommand>()
            .Map(command => command.IsId, request => string.IsNullOrWhiteSpace(request.Brand)
                                                     && string.IsNullOrWhiteSpace(request.Model));

        config.NewConfig<CustomerOrder, OrderResponse>()
            .MapWith(order => new OrderResponse(order.OrderId.Value, order.CentreId.Value));
        
        config.NewConfig<Customer, CustomerForListingResponse>()
            .Map(response => response.City, customer => customer.CityId)
            .Map(response => response.Orders, customer => customer.CustomerOrders);

        config.NewConfig<Customer, CustomerResponse>()
            .Map(response => response.City, customer => customer.CityId)
            .Map(response => response.Orders, customer => customer.CustomerOrders)
            .Map(response => response.Products, customer => customer.CustomerProducts.Select(cp => cp.Product));
        
        config.NewConfig<CustomerCreatedEvent, CustomerCreated>()
            .Map(response => response.City, customer => customer.CityId);
        
        config.NewConfig<CustomerAddressChangedEvent, CustomerAddressChanged>()
            .Map(response => response.City, customer => customer.CityId);
    }
}