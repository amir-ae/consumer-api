using Consumer.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Fixtures
{
    public class ConsumerDbContextFactory
    {
        public readonly TestConsumerDbContext ContextInstance;
        private readonly string _connectionString =
            "server=localhost; port=5432; timeout=15; pooling=True; minpoolsize=1; maxpoolsize=100; commandtimeout= 20; database=ConsumerTests; user id=postgres; password=T1VWLjZIofw60dVeYI2s";

        public ConsumerDbContextFactory()
        {
            var contextOptions = new DbContextOptionsBuilder<ConsumerDbContext>()
                .UseNpgsql(_connectionString, serverOptions =>
                {
                    serverOptions.MigrationsAssembly(typeof(ConsumerDbContext).Assembly.FullName);
                    serverOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                })
                .EnableSensitiveDataLogging()
                .Options;

            EnsureCreation(contextOptions);

            ContextInstance = new TestConsumerDbContext(contextOptions);
        }

        private void EnsureCreation(DbContextOptions<ConsumerDbContext> contextOptions)
        {
            using var context = new TestConsumerDbContext(contextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
