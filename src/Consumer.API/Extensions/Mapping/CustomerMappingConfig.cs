using Mapster;
using Consumer.Application.Customers.Commands.Update;
using Consumer.API.Contract.V1.Customers.Requests;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.API.Contract.V1.Customers.Responses.Events;
using Consumer.Application.Customers.Commands.AddOrder;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Application.Products.Commands.Create;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.API.Extensions.Mapping;

public class CustomerMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<PostCustomerRequest, CreateCustomerCommand>()
            .Map(command => command.AppUserId, request => new AppUserId(request.PostBy))
            .Map(command => command.CityId, request => new CityId(request.CityId))
            .Map(command => command.ProductIds, request => request.ProductIds == null ? 
                null : request.ProductIds.Select(p => new ProductId(p)).ToHashSet())
            .Map(command => command.Role, request => request.Role == null ? 
                (CustomerRole?)null : (CustomerRole)request.Role)
            .Map(command => command.CreatedAt, request => request.PostAt);

        config.NewConfig<PatchCustomerRequest, UpdateCustomerCommand>()
            .Map(command => command.AppUserId, request => new AppUserId(request.PatchBy))
            .Map(command => command.CityId, request => request.CityId == null ? 
                null : new CityId(request.CityId.Value))
            .Map(command => command.ProductIds, request => request.ProductIds == null ? 
                null : request.ProductIds.Select(id => new ProductId(id)).ToHashSet())
            .Map(command => command.Role, request => request.Role == null ? 
                (CustomerRole?)null : (CustomerRole)request.Role)
            .Map(command => command.UpdatedAt, request => request.PatchAt);

        config.NewConfig<PostCustomerOrderRequest, AddOrderCommand>()
            .Map(command => command.Owner, request => request.Owner == null ? null : new CreateCustomerCommand(
                new AppUserId(request.PostBy),
                request.Owner.FirstName,
                request.Owner.MiddleName,
                request.Owner.LastName,
                request.Owner.PhoneNumber,
                new CityId(request.Owner.CityId),
                request.Owner.Address,
                new HashSet<ProductId> { new(request.Product.ProductId) },
                CustomerRole.Owner,
                request.PostAt))
            .Map(command => command.Dealer, request => request.Dealer == null ? null : new CreateCustomerCommand(
                new AppUserId(request.PostBy),
                request.Dealer.Name,
                string.Empty,
                string.Empty,
                request.Dealer.PhoneNumber,
                new CityId(request.Dealer.CityId),
                request.Dealer.Address,
                new HashSet<ProductId> { new(request.Product.ProductId) },
                CustomerRole.Dealer,
                request.PostAt))
            .Map(result => result.CustomerRole, request => (CustomerRole)request.CustomerRole)
            .Map(result => result.Product, request => new CreateProductCommand(
                new AppUserId(request.PostBy),
                new ProductId(request.Product.ProductId),
                request.Product.Brand,
                request.Product.Model,
                request.Product.SerialId == null ? null : new SerialId(request.Product.SerialId.Value),
                null,
                null,
                null,
                request.Product.DeviceType,
                request.Product.PanelModel,
                request.Product.PanelSerialNumber,
                request.Product.WarrantyCardNumber,
                request.Product.DateOfPurchase,
                request.Product.InvoiceNumber,
                request.Product.PurchasePrice,
                request.Product.IsUnrepairable,
                request.Product.DateOfDemandForCompensation,
                request.Product.DemanderFullName,
                request.PostAt))
            .Map(command => command.ProductCondition, request => new CreateProductConditionCommand(
                new CentreId(request.CentreId),
                request.ProductCondition.Completeness,
                request.ProductCondition.Appearance,
                request.ProductCondition.Malfunction,
                request.ProductCondition.Warranty,
                request.ProductCondition.EstimatedCost,
                request.PostAt));

        config.NewConfig<(Customer, Product), CustomerOrderResponse>()
            .Map(result => result.Customer, response => response.Item1)
            .Map(result => result.Product, response => response.Item2);

        config.NewConfig<Customer, CustomerForListingResponse>()
            .Map(response => response.CustomerId, customer => customer.Id.Value)
            .Map(response => response.City.CityId, customer => customer.CityId.Value)
            .Map(response => response.ProductIds, customer => customer.ProductIds.Select(p => p.Value))
            .Map(response => response.Role, @event => (int)@event.Role);
        
        config.NewConfig<Customer, CustomerResponse>()
            .Map(response => response.CustomerId, customer => customer.Id.Value)
            .Map(response => response.City.CityId, customer => customer.CityId.Value)
            .Map(response => response.ProductIds, customer => customer.ProductIds.Select(p => p.Value))
            .Map(response => response.CreatedBy, customer => customer.CreatedBy.Value)
            .Map(result => result.LastModifiedBy,
                source => source.LastModifiedBy == null ? (Guid?)null : source.LastModifiedBy.Value);

        config.NewConfig<CustomerCreatedEvent, CustomerCreated>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.City.CityId, @event => @event.CityId.Value)
            .Map(response => response.ProductIds,
                @event => @event.ProductIds == null ? null : @event.ProductIds.Select(p => p.Value))
            .Map(response => response.Role, @event => (int)@event.Role)
            .Map(response => response.CreatedBy, @event => @event.CreatedBy.Value);
        
        config.NewConfig<CustomerNameChangedEvent, CustomerNameChanged>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.ChangedBy, @event => @event.ChangedBy.Value);
        
        config.NewConfig<CustomerPhoneNumberChangedEvent, CustomerPhoneNumberChanged>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.ChangedBy, @event => @event.ChangedBy.Value);

        config.NewConfig<CustomerAddressChangedEvent, CustomerAddressChanged>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.City.CityId, @event => @event.CityId.Value)
            .Map(response => response.ChangedBy, @event => @event.ChangedBy.Value);
        
        config.NewConfig<CustomerProductAddedEvent, CustomerProductAdded>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.ProductAddedBy, @event => @event.ProductAddedBy.Value);
        
        config.NewConfig<CustomerProductRemovedEvent, CustomerProductRemoved>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.ProductId, @event => @event.ProductId.Value)
            .Map(response => response.ProductRemovedBy, @event => @event.ProductRemovedBy.Value);
        
        config.NewConfig<CustomerRoleChangedEvent, CustomerRoleChanged>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.Role, @event => (int)@event.Role)
            .Map(response => response.ChangedBy, @event => @event.ChangedBy.Value);
        
        config.NewConfig<CustomerActivatedEvent, CustomerActivated>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.ActivatedBy, @event => @event.ActivatedBy.Value);
        
        config.NewConfig<CustomerDeactivatedEvent, CustomerDeactivated>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.DeactivatedBy, @event => @event.DeactivatedBy.Value);
        
        config.NewConfig<CustomerDeletedEvent, CustomerDeleted>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.DeletedBy, @event => @event.DeletedBy.Value);
        
        config.NewConfig<CustomerUndeletedEvent, CustomerUndeleted>()
            .Map(response => response.CustomerId, @event => @event.CustomerId.Value)
            .Map(response => response.UndeletedBy, @event => @event.UndeletedBy.Value);
    }
}