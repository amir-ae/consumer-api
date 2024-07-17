using Consumer.Domain.Products;
using Consumer.Infrastructure.Common.Persistence.Configurations;
using Marten.Events.Projections;
using Marten;

namespace Consumer.Infrastructure.Products.Configurations;

public class MartenTableMetaDataProduct : MartenTableMetaDataBase
{
    protected override void SetSpecificTableMetaData(StoreOptions storeOptions)
    {
        storeOptions.Schema.For<Product>().Identity(x => x.AggregateId);
        storeOptions.Schema.For<Product>().UseNumericRevisions(true);
        storeOptions.Projections.Snapshot<Product>(SnapshotLifecycle.Inline);
    }
}