using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Products.ValueConverters;

public class OrderIdValueConverter : ValueConverter<OrderId, string>
{
    public OrderIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new OrderId(value),
            mappingHints
        ) { }
}