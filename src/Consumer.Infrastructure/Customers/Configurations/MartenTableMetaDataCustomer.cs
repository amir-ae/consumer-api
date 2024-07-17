using Consumer.Domain.Customers;
using Consumer.Infrastructure.Common.Persistence.Configurations;
using Consumer.Infrastructure.Customers.Projections;
using Marten;
using Marten.Events.Projections;

namespace Consumer.Infrastructure.Customers.Configurations;

public class MartenTableMetaDataCustomer : MartenTableMetaDataBase
{
    protected override void SetSpecificTableMetaData(StoreOptions storeOptions)
    {
        storeOptions.Schema.For<Customer>().Identity(x => x.AggregateId);
        storeOptions.Schema.For<Customer>().UseNumericRevisions(true);
        //storeOptions.Projections.Snapshot<Customer>(SnapshotLifecycle.Inline);
        storeOptions.Projections.Add<CustomerProjection>(ProjectionLifecycle.Inline);
    }
}