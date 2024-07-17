using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Products.ValueConverters;

public class CentreIdValueConverter : ValueConverter<CentreId, Guid>
{
    public CentreIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new CentreId(value),
            mappingHints
        ) { }
}