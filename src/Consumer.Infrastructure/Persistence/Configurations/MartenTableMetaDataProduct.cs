using Consumer.Domain.Products;
using Marten;
using Marten.Events.Projections;

namespace Consumer.Infrastructure.Persistence.Configurations;

public class MartenTableMetaDataProduct : MartenTableMetaDataBase
{
    protected override void SetSpecificTableMetaData(StoreOptions storeOptions)
    {
        storeOptions.Schema.For<Product>().Identity(x => x.AggregateId);
        storeOptions.Projections.Snapshot<Product>(SnapshotLifecycle.Inline);
    }
}