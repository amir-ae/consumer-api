using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Products.ValueConverters;

public class ProductOrdersValueConverter : ValueConverter<HashSet<ProductOrder>, string>
{
    public ProductOrdersValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            keys => string.Join(';', keys.Select(key => $"{key.OrderId.Value},{key.CentreId.Value}")),
            value => 
                new HashSet<ProductOrder>(value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(parts => new ProductOrder { OrderId = new OrderId(parts[0]), CentreId = new CentreId(new Guid(parts[1]))})
        ),
            mappingHints)
    { }
}