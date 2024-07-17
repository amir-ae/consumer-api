using Consumer.Domain.Common.Entities;
using Consumer.Domain.Customers;
using Consumer.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Infrastructure.Common.Persistence;

public class ConsumerDbContext : DbContext
{
    public static readonly string DefaultSchema = Environment.GetEnvironmentVariable("SchemaName") ?? "consumer";

    public ConsumerDbContext(
        DbContextOptions<ConsumerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CustomerProduct> CustomerProducts => Set<CustomerProduct>();
    public DbSet<CustomerOrder> CustomerOrders => Set<CustomerOrder>();
    public DbSet<ProductOrder> ProductOrders => Set<ProductOrder>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ConsumerDbContext).Assembly);

        builder.HasDefaultSchema(DefaultSchema);

        base.OnModelCreating(builder);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken ct = default)
    {
        var result = await SaveChangesAsync(ct);

        if (result > 0) return true;
        
        return false;
    }
}