using Marten;

namespace Consumer.Infrastructure.Common.Persistence.Configurations;

public interface IMartenTableMetaData
{
    void SetTableMetaData(StoreOptions storeOptions);
}