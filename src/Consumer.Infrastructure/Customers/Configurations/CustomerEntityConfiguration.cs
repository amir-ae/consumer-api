using Consumer.Domain.Customers;
using Consumer.Infrastructure.Common.Extensions;
using Consumer.Infrastructure.Common.Persistence;
using Consumer.Infrastructure.Common.Persistence.ValueConverters;
using Consumer.Infrastructure.Customers.ValueConverters;
using Consumer.Infrastructure.Products.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consumer.Infrastructure.Customers.Configurations;

public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers", ConsumerDbContext.DefaultSchema);
        
        builder.OwnsOne(b => b.PhoneNumber, ownedNavigationBuilder => 
            PhoneNumberConfiguration.ConfigurePhoneNumber(ownedNavigationBuilder, nameof(Customer.PhoneNumber)));
        
        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Id)
            .HasColumnName(nameof(Customer.Id).ToSnakeCase())
            .ValueGeneratedOnAdd()
            .HasMaxLength(36)
            .HasConversion(new CustomerIdValueConverter());

        builder.Property(p => p.FirstName)
            .HasColumnName(nameof(Customer.FirstName).ToSnakeCase())
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.MiddleName)
            .HasColumnName(nameof(Customer.MiddleName).ToSnakeCase())
            .HasMaxLength(50);
        
        builder.Property(p => p.LastName)
            .HasColumnName(nameof(Customer.LastName).ToSnakeCase())
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(p => p.FullName)
            .HasColumnName(nameof(Customer.FullName).ToSnakeCase())
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.CityId)
            .HasColumnName(nameof(Customer.CityId).ToSnakeCase())
            .HasConversion(new CityIdValueConverter())
            .IsRequired();
        
        builder.Property(p => p.Address)
            .HasColumnName(nameof(Customer.Address).ToSnakeCase())
            .HasMaxLength(255)
            .IsRequired();
        
        builder.Property(p => p.Role)
            .HasColumnName(nameof(Customer.Role).ToSnakeCase())
            .HasConversion(new CustomerRoleValueConverter())
            .IsRequired();

        builder.Property(p => p.CustomerOrders)
            .HasColumnName(nameof(Customer.CustomerOrders).ToSnakeCase())
            .HasConversion(new CustomerOrdersValueConverter())
            .IsRequired()
            .Metadata.SetValueComparer(new OrdersValueComparer());
        
        builder.Property(p => p.ProductIds)
            .HasColumnName(nameof(Customer.ProductIds).ToSnakeCase())
            .HasConversion(new ProductIdsValueConverter())
            .IsRequired()
            .Metadata.SetValueComparer(new ProductIdsValueComparer());

        builder.Property(b => b.CreatedAt)
            .HasColumnName(nameof(Customer.CreatedAt).ToSnakeCase())
            .IsRequired();

        builder.Property(b => b.CreatedBy)
            .HasColumnName(nameof(Customer.CreatedBy).ToSnakeCase())
            .HasConversion(new AppUserIdValueConverter())
            .IsRequired();

        builder.Property(b => b.LastModifiedAt)
            .HasColumnName(nameof(Customer.LastModifiedAt).ToSnakeCase());

        builder.Property(b => b.LastModifiedBy)
            .HasColumnName(nameof(Customer.LastModifiedBy).ToSnakeCase())
            .HasConversion(new NullableAppUserIdValueConverter());

        builder
            .Property(m => m.AggregateId)
            .HasColumnName(nameof(Customer.AggregateId).ToSnakeCase())
            .HasMaxLength(36)
            .ValueGeneratedOnAdd();
        
        builder.Property(b => b.Version)
            .HasColumnName(nameof(Customer.Version).ToSnakeCase())
            .IsRequired();
        
        builder.Property(b => b.IsActive)
            .HasColumnName(nameof(Customer.IsActive).ToSnakeCase())
            .HasConversion(new BooleanValueConverter())
            .HasDefaultValue(true)
            .IsRequired();
        
        builder.Property(b => b.IsDeleted)
            .HasColumnName(nameof(Customer.IsDeleted).ToSnakeCase())
            .HasConversion(new BooleanValueConverter())
            .HasDefaultValue(false)
            .IsRequired();
    }
}