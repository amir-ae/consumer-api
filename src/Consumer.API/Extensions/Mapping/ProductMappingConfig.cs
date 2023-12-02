using Mapster;

using Consumer.API.Contract.V1.Products.Requests;
using Consumer.Application.Common.Commands;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Common.Entities;
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
                                                     && string.IsNullOrWhiteSpace(request.PhoneNumber) 
                                                     && request.CityId == null && string.IsNullOrWhiteSpace(request.Address));

        config.NewConfig<Dealer, UpsertCustomerCommand>()
            .Map(command => command.CustomerId, request => request.DealerId)
            .Map(command => command.FirstName, request => request.Name)
            .Map(command => command.Role, request => CustomerRole.Dealer)
            .Map(command => command.IsId, request => string.IsNullOrWhiteSpace(request.Name) 
                                                     && string.IsNullOrWhiteSpace(request.PhoneNumber) 
                                                     && request.CityId == null && string.IsNullOrWhiteSpace(request.Address));  
        
        config.NewConfig<Order, OrderResponse>()
            .MapWith(order => new OrderResponse(order.Id.Value, order.CentreId.Value));
    }
}