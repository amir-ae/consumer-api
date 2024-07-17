using Consumer.Domain.Common.Entities;
using Consumer.Infrastructure.Products.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consumer.Infrastructure.Common.Persistence.Configurations;

public class ProductOrderEntityConfiguration : IEntityTypeConfiguration<ProductOrder>
{
    public void Configure(EntityTypeBuilder<ProductOrder> builder)
    {
        builder
            .ToTable("product_order")
            .HasKey(po => new { po.ProductId, po.OrderId });

        builder
            .Property(po => po.ProductId)
            .HasColumnName("product_id")
            .HasMaxLength(50)
            .HasConversion(new ProductIdValueConverter());

        builder
            .Property(po => po.OrderId)
            .HasColumnName("order_id")
            .HasMaxLength(50)
            .HasConversion(new OrderIdValueConverter());
        
        builder
            .Property(po => po.CentreId)
            .HasColumnName("centre_id")
            .HasConversion(new CentreIdValueConverter())
            .IsRequired();
    }
}