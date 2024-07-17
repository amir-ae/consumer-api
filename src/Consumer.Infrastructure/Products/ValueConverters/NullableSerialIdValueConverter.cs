using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Products.ValueConverters;

public class NullableSerialIdValueConverter : ValueConverter<SerialId?, int?>
{
    public NullableSerialIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id == null ? null : id.Value,
            value => value.HasValue ? new SerialId(value.Value) : null,
            mappingHints
        ) { }
}