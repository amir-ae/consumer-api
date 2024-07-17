using Consumer.Infrastructure.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Consumer.API.Extensions.Database;

public static class DatabaseExtensions
{
    public static void CreateDatabase(this IApplicationBuilder app)
    {
        ApplyMigrations(app).GetAwaiter().GetResult();
    }
    
    private static async Task ApplyMigrations(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ConsumerDbContext>();
        
        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                Log.Information("Applying pending migrations...");
                await context.Database.MigrateAsync();
                Log.Information("Database is created/updated successfully.");
            }
            else
            {
                Log.Information("No pending migrations. Database is up-to-date.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while migrating the database: {ex.Message}");
        }
    }
}
    
