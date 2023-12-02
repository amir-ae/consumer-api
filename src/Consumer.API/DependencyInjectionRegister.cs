using Consumer.API.Extensions.Caching;
using Microsoft.Extensions.Options;
using Consumer.API.Extensions.ErrorHandling;
using Consumer.API.Extensions.Mapping;
using Consumer.API.Endpoints;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Consumer.API;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSingleton<IErrorHandler, ErrorHandler>();
        services.AddMappings();
        services.AddCaching();
        return services;
    }
}