using System.Reflection;
using System.Text.Json.Serialization;
using Consumer.Domain.Common.JsonConverters;
using Consumer.Domain.Customers;
using Consumer.Domain.Products;
using Consumer.Fixtures.Data;
using Consumer.Infrastructure.Persistence.Configurations;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace Consumer.Fixtures;

public class ConsumerApplicationFactory<Program> : WebApplicationFactory<Program> where Program : class
{
    private readonly string _connectionString =
        "server=localhost; port=5432; timeout=15; pooling=True; minpoolsize=1; maxpoolsize=100; commandtimeout= 20; database=ConsumerTests; user id=postgres; password=T1VWLjZIofw60dVeYI2s";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder
            .UseEnvironment("Testing")
            .ConfigureTestServices(services =>
            {
                var serializer = new Marten.Services.JsonNetSerializer();
                serializer.Customize(c =>
                {
                    c.Converters.Add(new StronglyTypedIdJsonConverter());
                    c.ContractResolver = new ResolvePrivateSetters();
                });
                services.AddMarten(storeOptions =>
                    {
                        storeOptions.DatabaseSchemaName = "ConsumerTests";
                        storeOptions.Connection(_connectionString);
                        storeOptions.AutoCreateSchemaObjects = AutoCreate.CreateOrUpdate;
                        storeOptions.Serializer(serializer);
                        storeOptions.CreateDatabasesForTenants(c =>
                        {
                            c.MaintenanceDatabase("host=localhost; user id=postgres; password=T1VWLjZIofw60dVeYI2s");
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
                        
                        storeOptions.Events.StreamIdentity = StreamIdentity.AsString;
                        storeOptions.Schema.For<Customer>().Identity(x => x.AggregateId);
                        storeOptions.Schema.For<Customer>().UseNumericRevisions(true);
                        storeOptions.Schema.For<Product>().Identity(x => x.AggregateId);
                        storeOptions.Schema.For<Product>().UseNumericRevisions(true);
                        storeOptions.Projections.Snapshot<Customer>(SnapshotLifecycle.Inline);
                        storeOptions.Projections.Snapshot<Product>(SnapshotLifecycle.Inline);
                    })
                    .InitializeWith(new InitialData(InitialDatasets.InitialData))
                    .UseLightweightSessions()
                    .AddAsyncDaemon(DaemonMode.Solo);
                
                services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
                services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            });
    }
}