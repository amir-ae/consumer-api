using Consumer.Domain.Products;
using Consumer.Infrastructure.Common.Persistence.Configurations;
using Consumer.Infrastructure.Products.Projections;
using Marten;
using Marten.Events.Projections;

namespace Consumer.Infrastructure.Products.Configurations;

public class MartenTableMetaDataProduct : MartenTableMetaDataBase
{
    protected override void SetSpecificTableMetaData(StoreOptions storeOptions)
    {
        storeOptions.Schema.For<Product>().Identity(x => x.AggregateId);
        storeOptions.Schema.For<Product>().UseNumericRevisions(true);
        //storeOptions.Projections.Snapshot<Product>(SnapshotLifecycle.Inline);
        storeOptions.Projections.Add<ProductProjection>(ProjectionLifecycle.Inline);
    }
}