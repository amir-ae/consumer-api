using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Products.ValueConverters;

public class ProductIdValueConverter : ValueConverter<ProductId, string>
{
    public ProductIdValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            id => id.Value,
            value => new ProductId(value),
            mappingHints
        ) { }
}