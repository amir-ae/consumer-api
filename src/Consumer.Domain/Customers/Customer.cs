using Consumer.Domain.Common.Entities;
using Consumer.Domain.Common.Models;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers;

public sealed record Customer : AggregateRoot<CustomerId, string>
{
    public string FirstName { get; private init; }
    public string? MiddleName { get; private init; }
    public string LastName { get; private init; }
    public string FullName { get; private init; }
    public string PhoneNumber { get; private init; }
    public CityId CityId { get; private init; }
    public string Address { get; private init; }
    public CustomerRole Role { get; private init; }
    public HashSet<ProductId> ProductIds { get; private init; }
    public HashSet<Product>? Products { get; private init; }
    public HashSet<Order> Orders { get; private init; }

    private Customer(
        CustomerId id,
        string firstName,
        string? middleName,
        string lastName,
        string fullName,
        string phoneNumber,
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
            created.CreatedBy,
            created.CreatedAt);
    
    public static async Task<Customer> CreateAsync(
        CustomerId id,
        string firstName,
        string? middleName,
        string lastName,
        string? fullName,
        string phoneNumber,
        CityId cityId,
        string address,
        CustomerRole? role,
        HashSet<ProductId>? productIds,
        HashSet<Order>? orders,
        AppUserId createdBy,
        DateTimeOffset? createdAt,
        Func<CustomerCreatedEvent, CancellationToken, Task<Customer>> create,
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

        return await create(@event, ct);
    }

    public Customer UpdateName(string? firstName, string? middleName, string? lastName, AppUserId updateBy, 
        DateTimeOffset? updateAt, Action<CustomerNameChangedEvent, int?> append, ref int? version, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrEmpty(firstName) && firstName != FirstName
                       || !string.IsNullOrEmpty(middleName) && middleName != MiddleName
                       || !string.IsNullOrEmpty(lastName) && lastName != LastName;
        if (!shouldUpdate) return this;
        
        var @event = new CustomerNameChangedEvent(
            Id,
            firstName ?? FirstName,
            middleName,
            lastName ?? LastName,
            null,
            updateBy,
            updateAt);
        
        append(@event, version++);
        return Apply(@event);
    }

    public Customer UpdatePhoneNumber(string? phoneNumber, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerPhoneNumberChangedEvent, int?> append, ref int? version, out bool shouldUpdate)
    {
        shouldUpdate = !string.IsNullOrEmpty(phoneNumber) && phoneNumber != PhoneNumber;
        if (!shouldUpdate) return this;
        
        var @event = new CustomerPhoneNumberChangedEvent(
            Id,
            phoneNumber!,
            updateBy,
            updateAt);
        
        append(@event, version++);
        return Apply(@event);
    }
    
    public Customer UpdateAddress(CityId? cityId, string? address, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerAddressChangedEvent, int?> append, ref int? version, out bool shouldUpdate)
    {
        shouldUpdate = cityId is not null && cityId != CityId
                       || !string.IsNullOrEmpty(address) && address != Address;
        if (!shouldUpdate) return this;
        
        var @event = new CustomerAddressChangedEvent(
            Id,
            cityId ?? CityId,
            address ?? Address,
            updateBy,
            updateAt);
        
        append(@event, version++);
        return Apply(@event);
    }
    
    public Customer UpdateRole(CustomerRole? role, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerRoleChangedEvent, int?> append, ref int? version, out bool shouldUpdate)
    {
        shouldUpdate = role is not null && role != Role;
        if (!shouldUpdate) return this;
        
        var @event = new CustomerRoleChangedEvent(
            Id,
            role!.Value,
            updateBy,
            updateAt);
        
        append(@event, version++);
        return Apply(@event);
    }
    
    public Customer AddOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerOrderAddedEvent, int?> append, ref int? version, out bool shouldUpdate)
    {
        shouldUpdate = orders is not null && !orders.SetEquals(Orders);
        if (!shouldUpdate) return this;
        
        var customer = this;
        var ordersToAdd = orders!.Except(Orders);
        foreach (var orderId in ordersToAdd)
        {
            var @event = new CustomerOrderAddedEvent(
                Id,
                orderId,
                updateBy,
                updateAt);
            
            append(@event, version++);
            customer = Apply(@event);
        }

        return customer;
    }
    
    public Customer RemoveOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<CustomerOrderRemovedEvent, int?> append, ref int? version, out bool shouldUpdate)
    {
        shouldUpdate = orders is not null && !orders.SetEquals(Orders);
        if (!shouldUpdate) return this;
        
        var customer = this;
        var ordersToRemove = Orders.Except(orders!);
        foreach (var orderId in ordersToRemove)
        {
            var @event = new CustomerOrderRemovedEvent(
                Id,
                orderId,
                updateBy,
                updateAt);
            
            append(@event, version++);
            customer = Apply(@event);
        }

        return customer;
    }
    
    public Customer AddProduct(ProductId productId, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerProductAddedEvent, int?> append, ref int? version, out bool shouldUpdate)
    {
        shouldUpdate = !ProductIds.Contains(productId);
        if (!shouldUpdate) return this;
        
        var @event = new CustomerProductAddedEvent(
            Id,
            productId,
            updateBy,
            updateAt);
        
        append(@event, version++);
        return Apply(@event);
    }
    
    public Customer RemoveProduct(ProductId productId, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerProductRemovedEvent, int?> append, ref int? version, out bool shouldUpdate)
    {
        shouldUpdate = ProductIds.Contains(productId);
        if (!shouldUpdate) return this;
        
        var @event = new CustomerProductRemovedEvent(
            Id,
            productId,
            updateBy,
            updateAt);
        
        append(@event, version++);
        return Apply(@event);
    }
    
    public Customer Activate(AppUserId activateBy, Action<CustomerActivatedEvent, int?> append)
    {
        if (IsActive) return this;
        
        var @event = new CustomerActivatedEvent(Id, activateBy);
        
        append(@event, null);
        return Apply(@event);
    }
    
    public Customer Deactivate(AppUserId deactivateBy, Action<CustomerDeactivatedEvent, int?> append)
    {
        if (!IsActive) return this;
        
        var @event = new CustomerDeactivatedEvent(Id, deactivateBy);
        
        append(@event, null);
        return Apply(@event);
    }
    
    public Customer Delete(AppUserId deleteBy, Action<CustomerDeletedEvent, int?> append)
    {
        if (IsDeleted) return this;
        
        var @event = new CustomerDeletedEvent(Id, deleteBy);
        
        append(@event, null);
        return Apply(@event);
    }
    
    public Customer Undelete(AppUserId undeleteBy, Action<CustomerUndeletedEvent, int?> append)
    {
        if (!IsDeleted) return this;
        
        var @event = new CustomerUndeletedEvent(Id, undeleteBy);
        
        append(@event, null);
        return Apply(@event);
    }
    
    public Customer UpdateName(string? firstName, string? middleName, string? lastName, AppUserId updateBy,
        DateTimeOffset? updateAt, Action<CustomerNameChangedEvent, int?> append, ref int? version)
        => UpdateName(firstName, middleName, lastName, updateBy, updateAt, append, ref version, out _);
    public Customer UpdatePhoneNumber(string? phoneNumber, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerPhoneNumberChangedEvent, int?> append, ref int? version)
        => UpdatePhoneNumber(phoneNumber, updateBy, updateAt, append, ref version, out _);
    public Customer UpdateAddress(CityId? cityId, string? address, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerAddressChangedEvent, int?> append, ref int? version)
        => UpdateAddress(cityId, address, updateBy, updateAt, append, ref version, out _);
    public Customer UpdateRole(CustomerRole? role, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerRoleChangedEvent, int?> append, ref int? version)
        => UpdateRole(role, updateBy, updateAt, append, ref version, out _);
    public Customer AddOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<CustomerOrderAddedEvent, int?> append, ref int? version)
        => AddOrders(orders, updateBy, updateAt, append, ref version, out _);
    public Customer RemoveOrders(HashSet<Order>? orders, AppUserId updateBy, DateTimeOffset? updateAt, 
        Action<CustomerOrderRemovedEvent, int?> append, ref int? version)
        => RemoveOrders(orders, updateBy, updateAt, append, ref version, out _);
    public Customer AddProduct(ProductId productId, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerProductAddedEvent, int?> append, ref int? version)
        => AddProduct(productId, updateBy, updateAt, append, ref version, out _);
    public Customer RemoveProduct(ProductId productId, AppUserId updateBy, DateTimeOffset? updateAt,
        Action<CustomerProductRemovedEvent, int?> append, ref int? version)
        => RemoveProduct(productId, updateBy, updateAt, append, ref version, out _);

    public Customer SetProductIds(HashSet<ProductId> productIds)
    {
        if (productIds.SetEquals(ProductIds)) return this;
        return this with
        {
            ProductIds = productIds
        };
    }
    
    public Customer AddProducts(HashSet<Product>? products)
    {
        if (products is null || Products is not null && products.SetEquals(Products)) return this;
        return this with
        {
            Products = products
        };
    }
    
    public Customer Apply(CustomerNameChangedEvent changed) =>
        this with
        {
            FirstName = changed.FirstName,
            MiddleName = changed.MiddleName,
            LastName = changed.LastName,
            FullName = changed.FullName,
            LastModifiedAt = changed.ChangedAt,
            LastModifiedBy = changed.ChangedBy
        };

    public Customer Apply(CustomerPhoneNumberChangedEvent changed) =>
        this with
        {
            PhoneNumber = changed.PhoneNumber,
            LastModifiedAt = changed.ChangedAt,
            LastModifiedBy = changed.ChangedBy
        };
    
    public Customer Apply(CustomerAddressChangedEvent changed) =>
        this with
        {
            CityId = changed.CityId,
            Address = changed.Address,
            LastModifiedAt = changed.ChangedAt,
            LastModifiedBy = changed.ChangedBy
        };
    
    public Customer Apply(CustomerRoleChangedEvent changed) =>
        this with
        {
            Role = changed.Role,
            LastModifiedAt = changed.ChangedAt,
            LastModifiedBy = changed.ChangedBy
        };
    
    public Customer Apply(CustomerProductAddedEvent added)
    {
        ProductIds.Add(added.ProductId);
        return this with
        {
            LastModifiedAt = added.ProductAddedAt,
            LastModifiedBy = added.ProductAddedBy
        };
    }

    public Customer Apply(CustomerProductRemovedEvent removed)
    {
        ProductIds.Remove(removed.ProductId);
        return this with
        {
            LastModifiedAt = removed.ProductRemovedAt,
            LastModifiedBy = removed.ProductRemovedBy
        };
    }
    
    public Customer Apply(CustomerOrderAddedEvent added)
    {
        Orders.Add(added.Order);
        return this with
        {
            LastModifiedAt = added.OrderAddedAt,
            LastModifiedBy = added.OrderAddedBy
        };
    }

    public Customer Apply(CustomerOrderRemovedEvent removed)
    {
        Orders.Remove(removed.Order);
        return this with
        {
            LastModifiedAt = removed.OrderRemovedAt,
            LastModifiedBy = removed.OrderRemovedBy
        };
    }
    
    public Customer Apply(CustomerActivatedEvent activated) =>
        this with
        {
            IsActive = true,
            LastModifiedAt = activated.ActivatedAt,
            LastModifiedBy = activated.ActivatedBy
        };
    
    public Customer Apply(CustomerDeactivatedEvent deactivated) =>
        this with
        {
            IsActive = false,
            LastModifiedAt = deactivated.DeactivatedAt,
            LastModifiedBy = deactivated.DeactivatedBy
        };
    
    public Customer Apply(CustomerDeletedEvent deleted) =>
        this with
        {
            IsActive = false,
            IsDeleted = true,
            LastModifiedAt = deleted.DeletedAt,
            LastModifiedBy = deleted.DeletedBy
        };
    
    public Customer Apply(CustomerUndeletedEvent undeleted) =>
        this with
        {
            IsActive = true,
            IsDeleted = false,
            LastModifiedAt = undeleted.UndeletedAt,
            LastModifiedBy = undeleted.UndeletedBy
        };

#pragma warning disable CS8618
    public Customer() { }
#pragma warning restore CS8618
}