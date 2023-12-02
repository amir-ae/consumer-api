using Consumer.Domain.Customers;
using Marten;
using Marten.Events.Projections;

namespace Consumer.Infrastructure.Persistence.Configurations;

public class MartenTableMetaDataCustomer : MartenTableMetaDataBase
{
    protected override void SetSpecificTableMetaData(StoreOptions storeOptions)
    {
        storeOptions.Schema.For<Customer>().Identity(x => x.AggregateId);
        storeOptions.Projections.Snapshot<Customer>(SnapshotLifecycle.Inline);
    }
}