using System.Reflection;
using Mapster;
using MapsterMapper;

namespace Consumer.API.Extensions.Mapping;

public static class MappingExtensions
{
    public static IServiceCollection AddMappings(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Default.MapToConstructor(true);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        config.Scan(Assembly.GetExecutingAssembly());
        return services;
    }
}