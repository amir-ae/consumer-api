using Marten.Events.Projections.Flattened;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Infrastructure.Common.Extensions;

namespace Consumer.Infrastructure.Customers.Projections;

public class CustomerProjection : FlatTableProjection
{
    public CustomerProjection() : base("Customers", SchemaNameSource.EventSchema)
    {
        Options.TeardownDataOnRebuild = false;

        Table.AddColumn<string>(nameof(Customer.Id).ToSnakeCase()).AsPrimaryKey();
        Table.AddColumn<string>(nameof(Customer.AggregateId).ToSnakeCase());
        Table.AddColumn<string>(nameof(Customer.FirstName).ToSnakeCase());
        Table.AddColumn<string>(nameof(Customer.MiddleName).ToSnakeCase());
        Table.AddColumn<string>(nameof(Customer.LastName).ToSnakeCase());
        Table.AddColumn<string>(nameof(Customer.FullName).ToSnakeCase());
        Table.AddColumn<int>(nameof(Customer.CityId).ToSnakeCase());
        Table.AddColumn<string>(nameof(Customer.Address).ToSnakeCase());
        Table.AddColumn<string>(string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.Value).ToSnakeCase()));
        Table.AddColumn<string>(string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.CountryId).ToSnakeCase()));
        Table.AddColumn<string>(string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.CountryCode).ToSnakeCase()));
        Table.AddColumn<string>(string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.Description).ToSnakeCase())).AllowNulls();
        Table.AddColumn<int>(nameof(Customer.Role).ToSnakeCase());
        Table.AddColumn<string>(nameof(Customer.CustomerOrders).ToSnakeCase());
        Table.AddColumn<string>(nameof(Customer.ProductIds).ToSnakeCase());
        Table.AddColumn<DateTimeOffset>(nameof(Customer.CreatedAt).ToSnakeCase());
        Table.AddColumn<Guid>(nameof(Customer.CreatedBy).ToSnakeCase());
        Table.AddColumn<DateTimeOffset>(nameof(Customer.LastModifiedAt).ToSnakeCase());
        Table.AddColumn<Guid>(nameof(Customer.LastModifiedBy).ToSnakeCase());
        Table.AddColumn<int>(nameof(Customer.Version).ToSnakeCase());
        Table.AddColumn<int>(nameof(Customer.IsActive).ToSnakeCase()).DefaultValue(1);
        Table.AddColumn<int>(nameof(Customer.IsDeleted).ToSnakeCase()).DefaultValue(0);

        Project<CustomerCreatedEvent>(map =>
        {
            map.Map(x => x.FirstName, nameof(Customer.FirstName).ToSnakeCase());
            map.Map(x => x.MiddleName, nameof(Customer.MiddleName).ToSnakeCase());
            map.Map(x => x.LastName, nameof(Customer.LastName).ToSnakeCase());
            map.Map(x => x.FullName, nameof(Customer.FullName).ToSnakeCase());
            map.Map(x => x.PhoneNumber.Value, string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.Value).ToSnakeCase()));
            map.Map(x => x.PhoneNumber.CountryId.Value, string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.CountryId).ToSnakeCase()));
            map.Map(x => x.PhoneNumber.CountryCode, string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.CountryCode).ToSnakeCase()));
            map.Map(x => x.PhoneNumber.Description, string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.Description).ToSnakeCase()));
            map.Map(x => x.CityId.Value, nameof(Customer.CityId).ToSnakeCase());
            map.Map(x => x.Address, nameof(Customer.Address).ToSnakeCase());
            map.Map(x => x.Role.Value, nameof(Customer.Role).ToSnakeCase());
            map.Map(x => x.ProductIdsString, nameof(Customer.ProductIds).ToSnakeCase());
            map.Map(x => x.OrdersString, nameof(Customer.CustomerOrders).ToSnakeCase());
            map.Map(x => x.CreatedAt, nameof(Customer.CreatedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.CreatedBy).ToSnakeCase());
            map.Map(x => x.CustomerId.Value, nameof(Customer.AggregateId).ToSnakeCase());
            map.SetValue(nameof(Customer.Version).ToSnakeCase(), 1);
        });
        
        Project<CustomerNameChangedEvent>(map =>
        {
            map.Map(x => x.FirstName, nameof(Customer.FirstName).ToSnakeCase());
            map.Map(x => x.MiddleName, nameof(Customer.MiddleName).ToSnakeCase());
            map.Map(x => x.LastName, nameof(Customer.LastName).ToSnakeCase());
            map.Map(x => x.FullName, nameof(Customer.FullName).ToSnakeCase());
            map.Map(x => x.NameChangedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });

        Project<CustomerPhoneNumberChangedEvent>(map =>
        {
            map.Map(x => x.PhoneNumber.Value, string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.Value).ToSnakeCase()));
            map.Map(x => x.PhoneNumber.CountryId.Value, string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.CountryId).ToSnakeCase()));
            map.Map(x => x.PhoneNumber.CountryCode, string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.CountryCode).ToSnakeCase()));
            map.Map(x => x.PhoneNumber.Description, string.Concat(nameof(Customer.PhoneNumber).ToSnakeCase(), "_", nameof(Customer.PhoneNumber.Description).ToSnakeCase()));
            map.Map(x => x.PhoneNumberChangedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        Project<CustomerAddressChangedEvent>(map =>
        {
            map.Map(x => x.CityId.Value, nameof(Customer.CityId).ToSnakeCase());
            map.Map(x => x.Address, nameof(Customer.Address).ToSnakeCase());
            map.Map(x => x.AddressChangedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        Project<CustomerRoleChangedEvent>(map =>
        {
            map.Map(x => x.Role.Value, nameof(Customer.Role).ToSnakeCase());
            map.Map(x => x.RoleChangedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        Project<CustomerProductAddedEvent>(map =>
        {
            map.Map(x => x.ProductIdsString, nameof(Customer.ProductIds).ToSnakeCase());
            map.Map(x => x.ProductAddedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        Project<CustomerProductRemovedEvent>(map =>
        {
            map.Map(x => x.ProductIdsString, nameof(Customer.ProductIds).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        Project<CustomerOrderAddedEvent>(map =>
        {
            map.Map(x => x.OrdersString, nameof(Customer.CustomerOrders).ToSnakeCase());
            map.Map(x => x.OrderAddedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        Project<CustomerOrderRemovedEvent>(map =>
        {
            map.Map(x => x.OrdersString, nameof(Customer.CustomerOrders).ToSnakeCase());
            map.Map(x => x.OrderRemovedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        /*Project<CustomerActivatedEvent>(map =>
        {
            map.SetValue(nameof(Customer.IsActive).ToSnakeCase(), 1);
            map.Map(x => x.ActivatedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        Project<CustomerDeactivatedEvent>(map =>
        {
            map.SetValue(nameof(Customer.IsActive).ToSnakeCase(), 0);
            map.Map(x => x.DeactivatedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        Project<CustomerDeletedEvent>(map =>
        {
            map.SetValue(nameof(Customer.IsDeleted).ToSnakeCase(), 1);
            map.Map(x => x.DeletedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });
        
        Project<CustomerUndeletedEvent>(map =>
        {
            map.SetValue(nameof(Customer.IsDeleted).ToSnakeCase(), 0);
            map.Map(x => x.UndeletedAt, nameof(Customer.LastModifiedAt).ToSnakeCase());
            map.Map(x => x.Actor.Value, nameof(Customer.LastModifiedBy).ToSnakeCase());
            map.Increment(nameof(Customer.Version).ToSnakeCase());
        });*/
    }
}