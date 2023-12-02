using Consumer.API.Extensions.Caching;
using Microsoft.Extensions.Options;
using Consumer.API.Extensions.ErrorHandling;
using Consumer.API.Extensions.Mapping;
using Consumer.API.Extensions.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Consumer.API;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var schemaHelper = new SwaggerSchemaHelper();
            options.CustomSchemaIds(type => schemaHelper.SchemaId(type));
        });
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSingleton<IErrorHandler, ErrorHandler>();
        services.AddMappings();
        services.AddCaching();
        return services;
    }
}