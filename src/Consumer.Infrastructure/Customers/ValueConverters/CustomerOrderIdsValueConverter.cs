using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Consumer.Infrastructure.Customers.ValueConverters;

public class CustomerOrdersValueConverter : ValueConverter<HashSet<CustomerOrder>, string>
{
    public CustomerOrdersValueConverter(ConverterMappingHints? mappingHints = null)
        : base(
            keys => string.Join(';', keys.Select(key => $"{key.OrderId.Value},{key.CentreId.Value}")),
            value => 
                new HashSet<CustomerOrder>(value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        .Select(parts => new CustomerOrder { OrderId = new OrderId(parts[0]), CentreId = new CentreId(new Guid(parts[1]))})
        ),
            mappingHints)
    { }
}