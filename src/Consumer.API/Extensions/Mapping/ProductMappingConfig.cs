using Mapster;
using Consumer.Application.Products.Commands.Update;
using Consumer.Application.Products.Commands.Create;
using Consumer.API.Contract.V1.Products.Requests;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Application.Common.Commands;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.Entities;
using Product = Consumer.Domain.Products.Product;

namespace Consumer.API.Extensions.Mapping;

public class ProductMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PostProductRequest, CreateProductCommand>()
            .Map(command => command.AppUserId, request => request.PostBy)
            .Map(command => command.CreatedAt, request => request.PostAt);

        config.NewConfig<Owner, Customer>()
            .Map(command => command.CustomerId, request => request.OwnerId)
            .Map(command => command.Role, request => CustomerRole.Owner)
            .Map(command => command.IsId, request => string.IsNullOrWhiteSpace(request.FirstName)
                                                     && string.IsNullOrWhiteSpace(request.LastName)
                                                     && string.IsNullOrWhiteSpace(request.PhoneNumber) 
                                                     && request.CityId == null && string.IsNullOrWhiteSpace(request.Address));

        config.NewConfig<Dealer, Customer>()
            .Map(command => command.CustomerId, request => request.DealerId)
            .Map(command => command.FirstName, request => request.Name)
            .Map(command => command.Role, request => CustomerRole.Dealer)
            .Map(command => command.IsId, request => string.IsNullOrWhiteSpace(request.Name) 
                                                     && string.IsNullOrWhiteSpace(request.PhoneNumber) 
                                                     && request.CityId == null && string.IsNullOrWhiteSpace(request.Address));  

        config.NewConfig<PatchProductRequest, UpdateProductCommand>()
            .Map(command => command.AppUserId, request => new AppUserId(request.PatchBy))
            .Map(command => command.UpdatedAt, request => request.PatchAt);

        config.NewConfig<Order, ProductOrder>()
            .MapWith(order => new ProductOrder(order.OrderId.Value, order.CentreId.Value));

        config.NewConfig<Product, ProductForListingResponse>()
            .Map(response => response.ProductId, product => product.Id);

        config.NewConfig<Product, ProductResponse>()
            .Map(response => response.ProductId, product => product.Id);
    }
}