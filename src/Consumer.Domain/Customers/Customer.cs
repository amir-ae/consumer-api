using Consumer.Domain.Common.Entities;
using Consumer.Domain.Common.Models;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers;

public sealed class Customer : AggregateRoot<CustomerId, string>
{
    public string FirstName { get; private set; }
    public string? MiddleName { get; private set; }
    public string LastName { get; private set; }
    public string FullName { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public CityId CityId { get; private set; }
    public string Address { get; private set; }
    public CustomerRole Role { get; private set; }
    public HashSet<ProductId> ProductIds { get; private set; }
    public HashSet<Product>? Products { get; private set; }
    public HashSet<Order> Orders { get; private set; }

    public CustomerEventHandler<CustomerEvent> CustomerEventHandler { get; } = new();
    
    private Customer(
        CustomerId id,
        string firstName,
        string? middleName,
        string lastName,
        string fullName,
        PhoneNumber phoneNumber,
        CityId cityId,
        string address,
        CustomerRole role,
        HashSet<ProductId> productIds,
        HashSet<Order> orders,
        AppUserId createdBy,
        DateTimeOffset createdAt)
    {
        Id = id;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        Role = role;
        ProductIds = productIds;
        Products = null;
        Orders = orders;
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        IsActive = true;
        IsDeleted = false;
    }

    public static Customer Create(CustomerCreatedEvent created) =>
        new (created.CustomerId,
            created.FirstName,
            created.MiddleName,
            created.LastName,
            created.FullName,
            created.PhoneNumber,
            created.CityId,
            created.Address,
            created.Role,
            created.ProductIds,
            created.Orders,
            created.Actor,
            created.CreatedAt);

    public static async Task<Customer> CreateAsync(
        CustomerId id,
        string firstName,
        string? middleName,
        string lastName,
        string? fullName,
        PhoneNumber phoneNumber,
        CityId cityId,
        string address,
        CustomerRole? role,
        HashSet<ProductId>? productIds,
        HashSet<Order>? orders,
        AppUserId createdBy,
        DateTimeOffset? createdAt,
        Func<CustomerCreatedEvent, Customer>? create,
        Func<CustomerCreatedEvent, CancellationToken, Task<Customer>> createAsync,
        bool saveChanges = true,
        CancellationToken ct = default)
    {
        var @event = new CustomerCreatedEvent(
            id,
            firstName,
            middleName,
            lastName,
            fullName,
            phoneNumber,
            cityId,
            address,
            role,
            productIds,
            orders,
            createdBy,
            createdAt);

        if (!saveChanges && create is not null)
        {
            return create(@event);
        }
        
        return await createAsync(@event, ct);
    }

    public void UpdateName(string? firstName, string? middleName, string? lastName, AppUserId updateBy, 
        DateTimeOffset? updateAt, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrWhiteSpace(firstName) && firstName != FirstName
                       || middleName is not null && middleName != MiddleName
                       || !string.IsNullOrWhiteSpace(lastName) && lastName != LastName;
        if (!shouldUpdate) return;
        
        var @event = new CustomerNameChangedEvent(
            Id,
            firstName ?? FirstName,
            middleName,
            lastName ?? LastName,
            null,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }

    public void UpdatePhoneNumber(PhoneNumber? phoneNumber, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = phoneNumber is not null && phoneNumber != PhoneNumber;
        if (!shouldUpdate) return;

        var @event = new CustomerPhoneNumberChangedEvent(
            Id,
            phoneNumber!,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void UpdateAddress(CityId? cityId, string? address, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = cityId is not null && cityId != CityId
                       || !string.IsNullOrWhiteSpace(address) && address != Address;
        if (!shouldUpdate) return;
        
        var @event = new CustomerAddressChangedEvent(
            Id,
            cityId ?? CityId,
            address ?? Address,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void UpdateRole(CustomerRole? role, AppUserId updateBy, DateTimeOffset? updateAt, out bool shouldUpdate)
    {
        shouldUpdate = role is not null && role != Role;
        if (!shouldUpdate) return;
        
        var @event = new CustomerRoleChangedEvent(
            Id,
            role!,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void AddOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = orders is not null && !orders.SetEquals(Orders);
        if (!shouldUpdate) return;
        
        var customer = this;
        var ordersToAdd = orders!.Except(Orders);
        foreach (var orderId in ordersToAdd)
        {
            var @event = new CustomerOrderAddedEvent(
                Id,
                orderId,
                updateBy,
                updateAt);
            
            Apply(@event);
            CustomerEventHandler.RaiseEvent(@event);
        }
    }
    
    public void RemoveOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = orders is not null && !orders.SetEquals(Orders);
        if (!shouldUpdate) return;
        
        var ordersToRemove = Orders.Except(orders!);
        foreach (var orderId in ordersToRemove)
        {
            var @event = new CustomerOrderRemovedEvent(
                Id,
                orderId,
                updateBy,
                updateAt);
            
            Apply(@event);
            CustomerEventHandler.RaiseEvent(@event);
        }
    }
    
    public void AddProduct(ProductId productId, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = !ProductIds.Contains(productId);
        if (!shouldUpdate) return;
        
        var @event = new CustomerProductAddedEvent(
            Id,
            productId,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void RemoveProduct(ProductId productId, AppUserId updateBy, DateTimeOffset? updateAt)
    {
        var shouldUpdate = ProductIds.Contains(productId);
        if (!shouldUpdate) return;
        
        var @event = new CustomerProductRemovedEvent(
            Id,
            productId,
            updateBy,
            updateAt);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void Activate(AppUserId activateBy)
    {
        if (IsActive) return;
        
        var @event = new CustomerActivatedEvent(Id, activateBy);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void Deactivate(AppUserId deactivateBy)
    {
        if (!IsActive) return;
        
        var @event = new CustomerDeactivatedEvent(Id, deactivateBy);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void Delete(AppUserId deleteBy)
    {
        if (IsDeleted) return;
        
        var @event = new CustomerDeletedEvent(Id, deleteBy);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }
    
    public void Undelete(AppUserId undeleteBy)
    {
        if (!IsDeleted) return;
        
        var @event = new CustomerUndeletedEvent(Id, undeleteBy);
        
        Apply(@event);
        CustomerEventHandler.RaiseEvent(@event);
    }

    public void SetProductIds(HashSet<ProductId> productIds)
    {
        if (productIds.SetEquals(ProductIds)) return;
        ProductIds = productIds;
    }
    
    public void AddProducts(HashSet<Product>? products)
    {
        if (products is null || Products is not null && products.SetEquals(Products)) return;
        Products = products;
    }

    public void Apply(CustomerNameChangedEvent changed)
    {
        FirstName = changed.FirstName;
        MiddleName = changed.MiddleName;
        LastName = changed.LastName;
        FullName = changed.FullName;
        LastModifiedAt = changed.NameChangedAt;
        LastModifiedBy = changed.Actor;
    }

    public void Apply(CustomerPhoneNumberChangedEvent changed)
    {
        PhoneNumber = changed.PhoneNumber;
        LastModifiedAt = changed.PhoneNumberChangedAt;
        LastModifiedBy = changed.Actor;
    }

    public void Apply(CustomerAddressChangedEvent changed)
    {
        CityId = changed.CityId;
        Address = changed.Address;
        LastModifiedAt = changed.AddressChangedAt;
        LastModifiedBy = changed.Actor;
    }

    public void Apply(CustomerRoleChangedEvent changed)
    {
        Role = changed.Role;
        LastModifiedAt = changed.RoleChangedAt;
        LastModifiedBy = changed.Actor;
    }

    public void Apply(CustomerProductAddedEvent added)
    {
        ProductIds.Add(added.ProductId);
        LastModifiedAt = added.ProductAddedAt;
        LastModifiedBy = added.Actor;
    }

    public void Apply(CustomerProductRemovedEvent removed)
    {
        ProductIds.Remove(removed.ProductId);
        LastModifiedAt = removed.ProductRemovedAt;
        LastModifiedBy = removed.Actor;
    }
    
    public void Apply(CustomerOrderAddedEvent added)
    {
        Orders.Add(added.Order);
        LastModifiedAt = added.OrderAddedAt;
        LastModifiedBy = added.Actor;
    }

    public void Apply(CustomerOrderRemovedEvent removed)
    {
        Orders.Remove(removed.Order);
        LastModifiedAt = removed.OrderRemovedAt;
        LastModifiedBy = removed.Actor;
    }

    public void Apply(CustomerActivatedEvent activated)
    {
        IsActive = true;
        LastModifiedAt = activated.ActivatedAt;
        LastModifiedBy = activated.Actor;
    }

    public void Apply(CustomerDeactivatedEvent deactivated)
    {
        IsActive = false;
        LastModifiedAt = deactivated.DeactivatedAt;
        LastModifiedBy = deactivated.Actor;
    }

    public void Apply(CustomerDeletedEvent deleted)
    {
        IsActive = false;
        IsDeleted = true;
        LastModifiedAt = deleted.DeletedAt;
        LastModifiedBy = deleted.Actor;
    }

    public void Apply(CustomerUndeletedEvent undeleted)
    {
        IsActive = true;
        IsDeleted = false;
        LastModifiedAt = undeleted.UndeletedAt;
        LastModifiedBy = undeleted.Actor;
    }

#pragma warning disable CS8618
    public Customer() { }
#pragma warning restore CS8618
}