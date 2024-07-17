using Consumer.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Fixtures
{
    public class TestConsumerDbContext : ConsumerDbContext
    {
        public TestConsumerDbContext(DbContextOptions<ConsumerDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConsumerDbContext).Assembly);
            modelBuilder.HasDefaultSchema(DefaultSchema);
        }
    }
}
