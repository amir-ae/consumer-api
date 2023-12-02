using Mapster;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.Entities;
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

        config.NewConfig<int, CustomerRole>()
            .MapWith(role => (CustomerRole)role);
        
        config.NewConfig<int?, CustomerRole?>()
            .MapWith(role => role.HasValue ? (CustomerRole)role.Value : null);

        config.NewConfig<string, ProductId>()
            .MapWith(id => new ProductId(id));
        
        config.NewConfig<int, SerialId>()
            .MapWith(id => new SerialId(id));
        
        config.NewConfig<int?, SerialId?>()
            .MapWith(id => id.HasValue ? new SerialId(id.Value) : null);

        config.NewConfig<(string, Guid), Order>()
            .MapWith(id => new Order(new OrderId(id.Item1), new CentreId(id.Item2)));
        
        #endregion
        #region
        
        config.NewConfig<AppUserId, Guid>()
            .MapWith(id => id.Value);
        
        config.NewConfig<AppUserId?, Guid?>()
            .MapWith( id => id == null ? null : id.Value);

        config.NewConfig<CityId, int>()
            .MapWith(id => id.Value);

        config.NewConfig<CityId, CustomerCity>()
            .MapWith(id => new CustomerCity(id.Value, null, null, null));
        
        config.NewConfig<CustomerId, string>()
            .MapWith(id => id.Value);
        
        config.NewConfig<CustomerId?, string?>()
            .MapWith(id => id == null ? null : id.Value);
        
        config.NewConfig<CustomerRole, int>()
            .MapWith(role => (int)role);

        config.NewConfig<ProductId, string>()
            .MapWith(id => id.Value);
        
        config.NewConfig<CentreId, Guid>()
            .MapWith(id => id.Value);
        
        config.NewConfig<OrderId, string>()
            .MapWith(id => id.Value);
        
        config.NewConfig<SerialId, int>()
            .MapWith(id => id.Value);
        
        config.NewConfig<SerialId?, int?>()
            .MapWith(id => id == null ? null : id.Value);
        
        #endregion
    }
}