using JasperFx.Core;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Consumer.Fixtures;

[TestCaseOrderer("Consumer.Fixtures.PriorityOrderer", "Consumer.Fixtures")]
public class IntegrationTest : IClassFixture<ConsumerDbContextFactory>, IClassFixture<ConsumerApplicationFactory<Program>>
{
    protected readonly ConsumerApplicationFactory<Program> Factory;
    protected IDocumentStore DocumentStore => Factory.Services.GetRequiredService<IDocumentStore>();

    protected IntegrationTest(ConsumerApplicationFactory<Program> factory)
    {
        Factory = factory;
    }

    /// <summary>
    /// 1. Start generation of projections
    /// 2. Wait for projections to be projected
    /// </summary>
    protected async Task GenerateProjectionsAsync()
    {
        using var daemon = await DocumentStore.BuildProjectionDaemonAsync();
        await daemon.StartAllAsync();
        await daemon.WaitForNonStaleData(5.Seconds());
    }
        
    protected async Task ResetAllDataAsync()
    {
        await DocumentStore.Advanced.ResetAllData();
    }

    protected IDocumentSession OpenSession() => DocumentStore.LightweightSession();
}