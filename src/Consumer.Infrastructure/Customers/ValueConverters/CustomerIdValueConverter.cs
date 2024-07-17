using Consumer.Domain.Customers.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Customers.ValueConverters;

public class CustomerIdValueConverter : ValueConverter<CustomerId, string>
{
    public CustomerIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new CustomerId(value),
            mappingHints
        ) { }
}