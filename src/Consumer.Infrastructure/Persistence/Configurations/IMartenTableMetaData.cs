using Marten;

namespace Consumer.Infrastructure.Persistence.Configurations;

public interface IMartenTableMetaData
{
    void SetTableMetaData(StoreOptions storeOptions);
}