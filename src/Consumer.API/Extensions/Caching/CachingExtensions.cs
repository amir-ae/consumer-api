using Consumer.API.Contract.V1;
using Consumer.Application.Common.Interfaces.Services;
using StackExchange.Redis;

namespace Consumer.API.Extensions.Caching;

public static class CachingExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect("localhost"));
        services.AddOutputCache(options =>
        {
            options.AddBasePolicy(builder => builder
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Customers.List.Uri()))
                .Tag(nameof(Routes.Customers)).Expire(TimeSpan.FromSeconds(30)));
            options.AddBasePolicy(builder => builder
                .With(c => c.HttpContext.Request.Path.StartsWithSegments(Routes.Products.List.Uri()))
                .Tag(nameof(Routes.Products)).Expire(TimeSpan.FromSeconds(30)));
            options.AddPolicy("Auth", CacheAuthenticatedRequestsPolicy.Instance);
        });
        return services;
    }
    
    public static void SeedCache(this IApplicationBuilder app)
    {
        try
        {
            FetchCatalogResources(app).GetAwaiter().GetResult();
        }
        catch
        {
            // ignore
        }
    }
    
    private static async Task FetchCatalogResources(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var customerEnrichmentService = scope.ServiceProvider.GetRequiredService<IEnrichmentService>();
        await customerEnrichmentService.FetchCatalogResources();
    }
}