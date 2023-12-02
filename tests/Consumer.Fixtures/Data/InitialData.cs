using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Common.Entities;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;
using Marten;
using Marten.Schema;

namespace Consumer.Fixtures.Data;

public class InitialData: IInitialData
{
    private readonly object[] _initialData;

    public InitialData(params object[] initialData)
    {
        _initialData = initialData;
    }

    public async Task Populate(IDocumentStore store, CancellationToken ct)
    {
        await using var session = store.LightweightSession();
        
        if (session.Query<Customer>().Any() || session.Query<Product>().Any())
        {
            await store.Advanced.Clean.CompletelyRemoveAllAsync(ct);
        }

        foreach (var @event in _initialData)
        {
            switch (@event)
            {
                case CustomerCreatedEvent created:
                    session.Events.StartStream<Customer>(created.CustomerId.Value, created);
                    break;
                case ProductCreatedEvent created:
                    session.Events.StartStream<Product>(created.ProductId.Value, created);
                    break;
            }
        }
        await session.SaveChangesAsync(ct);
        
        foreach (var @event in _initialData)
        {
            switch (@event)
            {
                case CustomerDeletedEvent deleted:
                    session.Events.Append(deleted.CustomerId.Value, deleted);
                    break;
                case ProductDeletedEvent deleted:
                    session.Events.Append(deleted.ProductId.Value, deleted);
                    break;
            }
        }
        await session.SaveChangesAsync(ct);
    }
}

public static class InitialDatasets
{
    public static readonly object[] InitialData =
    {
        new CustomerCreatedEvent(
            new CustomerId("1"),
            "Rae",
            null,
            "Mccall",
            null,
            "(321) 682-8918",
            new CityId(1),
            "San Gregorio",
            null,
            new HashSet<ProductId> {new("A")},
            null,
            new AppUserId(Guid.NewGuid())),
        new CustomerCreatedEvent(
            new CustomerId("2"),
            "Aspen",
            null,
            "Coffey",
            null,
            "1-514-374-2937",
            new CityId(1),
            "Klerksdorp",
            null,
            new HashSet<ProductId> {new("B"), new("C")},
            null,
            new AppUserId(Guid.NewGuid())),
        new CustomerCreatedEvent(
            new CustomerId("3"),
            "Leonard",
            null,
            "Sandoval",
            null,
            "(576) 452-6868",
            new CityId(2),
            "Inírida",
            null,
            null,
            null,
            new AppUserId(Guid.NewGuid())),
        new CustomerDeletedEvent(
            new CustomerId("3"),
            new AppUserId(Guid.NewGuid())),
        new ProductCreatedEvent(
            new ProductId("A"),
            "TCL",
            "L40S60A",
            null,
            new CustomerId("1"),
            "Mccall Rae",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new HashSet<Order> { Order.Create(new OrderId("A"), new CentreId(Guid.Parse("14424963-25e4-4731-8501-461750d27037"))) },
            null,
            null,
            null,
            new AppUserId(Guid.NewGuid())),
        new ProductCreatedEvent(
            new ProductId("B"),
            "POLARLINE",
            "32PL13TC-SM",
            null,
            new CustomerId("2"),
            "Aspen Coffey",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new AppUserId(Guid.NewGuid())),
        new ProductCreatedEvent(
            new ProductId("C"),
            "STARWIND",
            "SW-LED40BA201",
            null,
            new CustomerId("2"),
            "Aspen Coffey",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            new AppUserId(Guid.NewGuid())),
        new ProductDeletedEvent(
            new ProductId("C"),
            new AppUserId(Guid.NewGuid()))
    };
}