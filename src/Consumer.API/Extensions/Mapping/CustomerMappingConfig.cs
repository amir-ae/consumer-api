using Mapster;
using Consumer.Application.Customers.Commands.Update;
using Consumer.API.Contract.V1.Customers.Requests;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.API.Contract.V1.Customers.Responses.Events;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Product = Consumer.Application.Common.Commands.Product;

namespace Consumer.API.Extensions.Mapping;

public class CustomerMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PostCustomerRequest, CreateCustomerCommand>()
            .Map(command => command.AppUserId, request => request.PostBy)
            .Map(command => command.CreatedAt, request => request.PostAt);

        config.NewConfig<CustomerProduct, Product>()
            .Map(command => command.IsId, request => string.IsNullOrWhiteSpace(request.Brand)
                                                     && string.IsNullOrWhiteSpace(request.Model));

        config.NewConfig<PatchCustomerRequest, UpdateCustomerCommand>()
            .Map(command => command.AppUserId, request => request.PatchBy)
            .Map(command => command.UpdatedAt, request => request.PatchAt);

        config.NewConfig<Customer, CustomerForListingResponse>()
            .Map(response => response.CustomerId, customer => customer.Id)
            .Map(response => response.City, customer => customer.CityId);

        config.NewConfig<Customer, CustomerResponse>()
            .Map(response => response.CustomerId, customer => customer.Id)
            .Map(response => response.City, customer => customer.CityId);
        
        config.NewConfig<CustomerCreatedEvent, CustomerCreated>()
            .Map(response => response.City, customer => customer.CityId);
        
        config.NewConfig<CustomerAddressChangedEvent, CustomerAddressChanged>()
            .Map(response => response.City, customer => customer.CityId);
    }
}