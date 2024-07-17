using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Products.ValueConverters;

public class ProductIdsValueConverter : ValueConverter<HashSet<ProductId>, string>
{
    public ProductIdsValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            ids => string.Join(',', ids.Select(id => id.Value)),
            value => new HashSet<ProductId>(value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(id => new ProductId(id))),
            mappingHints
        ) { }
}