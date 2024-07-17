using Consumer.Domain.Common.Entities;
using Consumer.Infrastructure.Customers.ValueConverters;
using Consumer.Infrastructure.Products.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consumer.Infrastructure.Common.Persistence.Configurations;

public class CustomerProductEntityConfiguration : IEntityTypeConfiguration<CustomerProduct>
{
    public void Configure(EntityTypeBuilder<CustomerProduct> builder)
    {
        builder
            .ToTable("customer_product")
            .HasKey(cp => new { cp.CustomerId, cp.ProductId });

        builder
            .Property(cp => cp.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(36)
            .HasConversion(new CustomerIdValueConverter());

        builder
            .Property(cp => cp.ProductId)
            .HasColumnName("product_id")
            .HasMaxLength(50)
            .HasConversion(new ProductIdValueConverter());;

        builder
            .HasOne(cp => cp.Customer)
            .WithMany(c => c.CustomerProducts)
            .HasForeignKey(cp => cp.CustomerId);

        builder
            .HasOne(cp => cp.Product)
            .WithMany(p => p.ProductCustomers)
            .HasForeignKey(cp => cp.ProductId);
    }
}