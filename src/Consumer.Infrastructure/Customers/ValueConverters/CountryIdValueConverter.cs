using Consumer.Domain.Customers.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Customers.ValueConverters;

public class CountryIdValueConverter : ValueConverter<CountryId, string>
{
    public CountryIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new CountryId(value),
            mappingHints
        ) { }
}