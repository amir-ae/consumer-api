using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Products.ValueConverters;

public class OrderIdsValueConverter : ValueConverter<HashSet<OrderId>, string>
{
    public OrderIdsValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            ids => string.Join(',', ids.Select(id => id.Value)),
            value => new HashSet<OrderId>(value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(id => new OrderId(id))),
            mappingHints
        ) { }
}