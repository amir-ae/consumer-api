using Consumer.Domain.Products;
using Marten;
using Marten.Events.Projections;

namespace Consumer.Infrastructure.Persistence.Configurations;

public class MartenTableMetaDataProduct : MartenTableMetaDataBase
{
    protected override void SetSpecificTableMetaData(StoreOptions storeOptions)
    {
        storeOptions.Schema.For<Product>().Identity(x => x.AggregateId);
        storeOptions.Schema.For<Product>().UseNumericRevisions(true);
        storeOptions.Schema.For<Product>()
            .Index(x => x.OwnerId!)
            .Index(x => x.DealerId!)
            .Index(x => x.CreatedAt)
            .Index(x => x.LastModifiedAt!);
        storeOptions.Projections.Snapshot<Product>(SnapshotLifecycle.Inline);
    }
}