using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Amazon;
using Amazon.SQS;
using Catalog.API.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Common.Configurations;
using Consumer.Domain.Common.JsonConverters;
using Consumer.Infrastructure.Authentication;
using Consumer.Infrastructure.Authentication.Policies;
using Consumer.Infrastructure.Persistence.Configurations;
using Consumer.Infrastructure.Persistence.Repositories;
using Consumer.Infrastructure.Services;
using Marten;
using Marten.Events.Daemon.Resiliency;
using MassTransit;
using Microsoft.AspNetCore.Http.Json;
using Polly;
using Polly.Retry;
using Weasel.Core;

namespace Consumer.Infrastructure;

public static class DependencyInjectionRegister
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services
            .AddAuth(config)
            .AddPersistence(config);
        
        services.AddLazyCache();
        services.AddSingleton<IEnrichmentService, EnrichmentService>();
        services.AddSingleton<IOrderingService, OrderingService>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddHttpClient<ICatalogClient>()
            //.AddPolicyHandler(HttpClientPolicies.RetryPolicy())
            .AddPolicyHandler(HttpClientPolicies.CircuitBreakerPolicy())
            .AddTypedClient<ICatalogClient>((hc, sp) 
                => new CatalogClient(hc.Configure(ApiEndpoints.CatalogApi, config, sp)));

        services.Configure<EventBusSettings>(config.GetSection(EventBusSettings.Key));
        services.Configure<QueueSettings>(config.GetSection(QueueSettings.Key));
        
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
        services.AddSingleton<ISqsMessenger, SqsMessenger>();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }

    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var serializer = new Marten.Services.JsonNetSerializer();
        serializer.Customize(c =>
        {
            c.Converters.Add(new StronglyTypedIdJsonConverter());
            c.ContractResolver = new ResolvePrivateSetters();
        });

        var defaultConnection = configuration.GetConnectionString("Default")!;
        var maintenanceConnection = configuration.GetConnectionString("Maintenance")!;
        AWSConfigs.AWSRegion = "eu-north-1";

        services.AddMarten(storeOptions =>
        {
            var schemaName = Environment.GetEnvironmentVariable("SchemaName") ?? "Consumer";
            storeOptions.DatabaseSchemaName = schemaName;
            storeOptions.Connection(defaultConnection);
            storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
            storeOptions.Serializer(serializer);
            storeOptions.ConfigurePolly(o => o.AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 2,
            }));
            storeOptions.CreateDatabasesForTenants(c =>
            {
                c.MaintenanceDatabase(maintenanceConnection);
                c.ForTenant()
                    .CheckAgainstPgDatabase()
                    .WithOwner("postgres")
                    .WithEncoding("UTF-8")
                    .ConnectionLimit(-1);
            });

            var stuff = Assembly.GetExecutingAssembly()
                .DefinedTypes.Where(type => type.IsSubclassOf(typeof(MartenTableMetaDataBase))).ToList();

            foreach (Type type in stuff)
            {
                IMartenTableMetaData temp = (IMartenTableMetaData)Activator.CreateInstance(type)!;
                temp.SetTableMetaData(storeOptions);
            }
        })
            .ApplyAllDatabaseChangesOnStartup()
            .UseLightweightSessions()
            .AddAsyncDaemon(DaemonMode.Solo);
        
        services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
    }

    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings();
        configuration.Bind(JwtSettings.SectionName, jwtSettings);

        services.AddSingleton(Options.Create(jwtSettings));

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key))
            });

        services.AddAuthorization();

        return services;
    }
    
    private static HttpClient Configure(this HttpClient hc, string baseAddress, IConfiguration config, IServiceProvider sp)
    {
        hc.BaseAddress = new Uri(config.GetValue<string>(baseAddress)!);
        hc.DefaultRequestHeaders.Add(ApiKeyConstants.HeaderName, config.GetValue<string>(ApiKeyConstants.SectionName));
        var request = sp.GetService<IHttpContextAccessor>()?.HttpContext?.Request;
        if (request is not null)
        {
            _ = AuthenticationHeaderValue.TryParse(request.Headers.Authorization, out var headerValue);
            hc.DefaultRequestHeaders.Authorization = headerValue;
        }
        return hc;
    }
}