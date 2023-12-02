using Mapster;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.API.Extensions.Mapping;

public class ValueObjectMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        #region
        
        config.NewConfig<Guid, AppUserId>()
            .MapWith(id => new AppUserId(id));
        
        config.NewConfig<int, CityId>()
            .MapWith(id => new CityId(id));
        
        config.NewConfig<int?, CityId?>()
            .MapWith(id => id.HasValue ? new CityId(id.Value) : null);
        
        config.NewConfig<string, CustomerId>()
            .MapWith(id => new CustomerId(id));
        
        config.NewConfig<string?, CustomerId?>()
            .MapWith(id => !string.IsNullOrWhiteSpace(id) ? new CustomerId(id) : null);

        config.NewConfig<string, ProductId>()
            .MapWith(id => new ProductId(id));
        
        config.NewConfig<int, SerialId>()
            .MapWith(id => new SerialId(id));
        
        config.NewConfig<int?, SerialId?>()
            .MapWith(id => id.HasValue ? new SerialId(id.Value) : null);

        config.NewConfig<string, OrderId>()
            .MapWith(id => new OrderId(id));
        
        config.NewConfig<Guid, CentreId>()
            .MapWith(id => new CentreId(id));
        
        #endregion
        #region
        
        config.NewConfig<AppUserId, Guid>()
            .MapWith(id => id.Value);
        
        config.NewConfig<AppUserId?, Guid?>()
            .MapWith( id => id == null ? null : id.Value);

        config.NewConfig<CityId, int>()
            .MapWith(id => id.Value);

        config.NewConfig<CityId, City>()
            .MapWith(id => new City(id.Value, null, null, null));
        
        config.NewConfig<CustomerId, string>()
            .MapWith(id => id.Value);
        
        config.NewConfig<CustomerId?, string?>()
            .MapWith(id => id == null ? null : id.Value);

        config.NewConfig<ProductId, string>()
            .MapWith(id => id.Value);

        config.NewConfig<OrderId, string>()
            .MapWith(id => id.Value);
        
        config.NewConfig<CentreId, Guid>()
            .MapWith(id => id.Value);
        
        config.NewConfig<SerialId, int>()
            .MapWith(id => id.Value);
        
        config.NewConfig<SerialId?, int?>()
            .MapWith(id => id == null ? null : id.Value);
        
        #endregion
    }
}