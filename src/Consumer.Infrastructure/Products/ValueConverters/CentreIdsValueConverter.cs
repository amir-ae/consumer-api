using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Products.ValueConverters;

public class CentreIdsValueConverter : ValueConverter<HashSet<CentreId>, string>
{
    public CentreIdsValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            ids => string.Join(',', ids.Select(id => id.Value)),
            value => new HashSet<CentreId>(value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(id => new CentreId(new Guid(id)))),
            mappingHints
        ) { }
}