using Mapster;

using Consumer.API.Contract.V1.Products.Requests;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Application.Common.Commands;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products;
using OrderResponse = Consumer.API.Contract.V1.Common.Order;

namespace Consumer.API.Extensions.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Owner, UpsertCustomerCommand>()
            .Map(command => command.CustomerId, request => request.OwnerId)
            .Map(command => command.Role, request => CustomerRole.Owner)
            .Map(command => command.IsId, request => string.IsNullOrWhiteSpace(request.FirstName)
                                                     && string.IsNullOrWhiteSpace(request.LastName)
                                                     && request.PhoneNumber == null 
                                                     && request.CityId == null && string.IsNullOrWhiteSpace(request.Address));

        config.NewConfig<Dealer, UpsertCustomerCommand>()
            .Map(command => command.CustomerId, request => request.DealerId)
            .Map(command => command.FirstName, request => request.Name)
            .Map(command => command.Role, request => CustomerRole.Dealer)
            .Map(command => command.IsId, request => string.IsNullOrWhiteSpace(request.Name) 
                                                     && request.PhoneNumber == null
                                                     && request.CityId == null && string.IsNullOrWhiteSpace(request.Address));  
        
        config.NewConfig<ProductOrder, OrderResponse>()
            .MapWith(order => new OrderResponse(order.OrderId.Value, order.CentreId.Value));
        
        config.NewConfig<Product, ProductForListingResponse>()
            .Map(response => response.Orders, customer => customer.ProductOrders);
            
        config.NewConfig<Product, ProductResponse>()
            .Map(response => response.Orders, customer => customer.ProductOrders)
            .Map(response => response.Owner, product => product.ProductCustomers
                .FirstOrDefault(pc => pc.Customer != null && pc.Customer.Id == product.OwnerId))
            .Map(response => response.Dealer, product => product.ProductCustomers
            .FirstOrDefault(pc => pc.Customer != null && pc.Customer.Id == product.DealerId));
    }
}