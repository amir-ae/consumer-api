using Consumer.Domain.Common.Models;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Customers;

public sealed record Customer : AggregateRoot<CustomerId, string>
{
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public CityId CityId { get; set; }
    public string Address { get; set; }
    public CustomerRole Role { get; set; }
    public HashSet<ProductId> ProductIds { get; set; }
    public HashSet<Product>? Products { get; set; }

    private Customer(
        CustomerId id,
        string firstName,
        string? middleName,
        string lastName,
        string fullName,
        string phoneNumber,
        CityId cityId,
        string address,
        HashSet<ProductId>? productIds,
        CustomerRole role,
        DateTimeOffset? createdAt,
        AppUserId createdBy)
    {
        Id = id;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        ProductIds = productIds ?? new();
        Role = role;
        CreatedAt = createdAt ?? DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
        IsDeleted = false;
        IsActive = true;
    }

    public static Customer Create(CustomerCreatedEvent created) =>
        new(created.CustomerId,
            created.FirstName,
            created.MiddleName,
            created.LastName,
            created.FullName,
            created.PhoneNumber,
            created.CityId,
            created.Address,
            created.ProductIds,
            created.Role,
            created.CreatedAt,
            created.CreatedBy);

    public Customer Apply(CustomerNameChangedEvent changed)
    {
        return this with
        {
            FirstName = changed.FirstName,
            MiddleName = changed.MiddleName,
            LastName = changed.LastName,
            FullName = changed.FullName,
            LastModifiedAt = changed.ChangedAt,
            LastModifiedBy = changed.ChangedBy
        };
    }

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