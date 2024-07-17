using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Common.Persistence.ValueConverters;

public class BooleanValueConverter : ValueConverter<bool, int>
{
    public BooleanValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id ? 1 : 0,
            value => value != 0,
            mappingHints
        ) { }
}