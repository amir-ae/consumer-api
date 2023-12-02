using Catalog.API.Client;
using Catalog.API.Contract.V1.Cities.Responses;
using Catalog.API.Contract.V1.Serials.Responses;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.API.Contract.V1.Customers.Responses.Events;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Application.Common.Interfaces.Services;
using LazyCache;

namespace Consumer.Infrastructure.Services;

public class EnrichmentService : IEnrichmentService
{
    private readonly ICatalogClient _client;
    private readonly IAppCache _cache;

    public EnrichmentService(ICatalogClient client, IAppCache cache)
    {
        _client = client;
        _cache = cache;
    }
    public IEnumerable<CityResponse>? Cities { get; private set; }
    public IEnumerable<SerialForListingResponse>? Serials { get; private set; }

    public async ValueTask<CustomerResponse> EnrichCustomerResponse(CustomerResponse customer, CancellationToken ct = default)
    {
        var city = await City(customer.City.CityId, ct);
        return Map(customer, city);
    }

    public async ValueTask<CustomerForListingResponse> EnrichCustomerForListingResponse(CustomerForListingResponse customer, CancellationToken ct = default)
    {
        var city = await City(customer.City.CityId, ct);
        return Map(customer, city);
    }

    public async ValueTask<ProductResponse> EnrichProductResponse(ProductResponse product, CancellationToken ct = default)
    {
        if (product.Owner is not null)
        {
            product = product with { Owner = await EnrichCustomerForListingResponse(product.Owner, ct) };
        }
        if (product.Dealer is not null)
        {
            product = product with { Dealer = await EnrichCustomerForListingResponse(product.Dealer, ct) };
        }
        
        if (product.SerialId.HasValue)
        {
            var serial = await Serial(product.SerialId.Value, ct);
            return Map(product, serial);
        }

        return product;
    }
    
    public async ValueTask<CustomerEventsResponse> EnrichCustomerEvents(CustomerEventsResponse customerEvents, CancellationToken ct = default)
    {
        return customerEvents with
        {
            CustomerCreatedEvent = await EnrichCustomerCreatedEvent(customerEvents.CustomerCreatedEvent, ct),
            CustomerAddressChangedEvents = await EnrichCustomerAddressChangedEvents(customerEvents.CustomerAddressChangedEvents, ct)
        };
    }

    public async ValueTask<CustomerCreated> EnrichCustomerCreatedEvent(CustomerCreated customerCreatedEvent, CancellationToken ct = default)
    {
        var city = await City(customerCreatedEvent.City.CityId, ct);
        return Map(customerCreatedEvent, city);
    }
    
    public async ValueTask<IList<CustomerAddressChanged>> EnrichCustomerAddressChangedEvents(
        IList<CustomerAddressChanged> customerAddressChangedEvents, CancellationToken ct = default)
    {
        if (!customerAddressChangedEvents.Any()) return customerAddressChangedEvents;
        
        var tasks = customerAddressChangedEvents
            .Select(async @event => await Map(@event, City(@event.City.CityId, ct)));
        
        return (await Task.WhenAll(tasks)).ToList();
    }

    public async Task FetchCatalogResources(CancellationToken ct = default)
    {
        var citiesTask = _client.City.List(null, ct);
        var serialsTask = _client.Serial.List(null, ct);

        await Task.WhenAll(citiesTask, serialsTask);

        var citiesResult = await citiesTask;
        var serialsResult = await serialsTask;

        if (!citiesResult.IsError)
        {
            Cities = citiesResult.Value;
            //_cache.Add("cities", Cities);
        }
        if (!serialsResult.IsError)
        {
            Serials = serialsResult.Value;
            //_cache.Add("serials", Serials);
        }
    }
    
    public async Task FetchCities(CancellationToken ct = default)
    {
        var result = await _client.City.List(null, ct);
        if (!result.IsError)
        {
            Cities = result.Value;
            //_cache.Add("cities", Cities);
        }
    }
    
    public async Task FetchSerials(CancellationToken ct = default)
    {
        var result = await _client.Serial.List(null, ct);
        if (!result.IsError)
        {
            Serials = result.Value;
            //_cache.Add("serials", Serials);
        }
    }

    private void CachedCities()
    {
        Cities = _cache.Get<List<CityResponse>>("cities");
    }
    
    private void CachedSerials()
    {
        Serials = _cache.Get<List<SerialForListingResponse>>("serials");
    }

    private async ValueTask<CityResponse?> City(int cityId, CancellationToken ct = default)
    {
        var city = Cities?.FirstOrDefault(c => c.Id == cityId);
        if (city is not null) return city;

        //CachedCities();
        //city = Cities?.FirstOrDefault(c => c.CityId == cityId);
        //if (city is not null) return city;
        
        await FetchCities(ct);
        return Cities?.FirstOrDefault(c => c.Id == cityId);
    }

    private async ValueTask<SerialForListingResponse?> Serial(int serialId, CancellationToken ct = default)
    {
        var serial = Serials?.FirstOrDefault(s => s.Id == serialId);
        if (serial is not null) return serial;

        //CachedSerials();
        //serial = Serials?.FirstOrDefault(s => s.SerialId == serialId);
        //if (serial is not null) return serial;
        
        await FetchSerials(ct);
        return Serials?.FirstOrDefault(s => s.Id == serialId);
    }
    
    private static CustomerResponse Map(CustomerResponse customer, CityResponse? city)
    {
        if (city is null) return customer;
        return customer with
        {
            City = new City(
                customer.City.CityId,
                city.Name,
                city.Oblast,
                city.Code)
        };
    }
    
    private static CustomerForListingResponse Map(CustomerForListingResponse customer, CityResponse? city)
    {
        if (city is null) return customer;
        return customer with
        {
            City = new City(
                customer.City.CityId,
                city.Name,
                city.Oblast,
                city.Code)
        };
    }
    
    private static CustomerCreated Map(CustomerCreated @event, CityResponse? city)
    {
        if (city is null) return @event;
        return @event with
        {
            City = new City(
                @event.City.CityId,
                city.Name,
                city.Oblast,
                city.Code)
        };
    }

    private static async ValueTask<CustomerAddressChanged> Map(CustomerAddressChanged @event, ValueTask<CityResponse?> cityTask)
    {
        var city = await cityTask;
        if (city is null) return @event;
        return @event with
        {
            City = new City(
                @event.City.CityId,
                city.Name,
                city.Oblast,
                city.Code)
        };
    }
    
    private static ProductResponse Map(ProductResponse product, SerialForListingResponse? serial)
    {
        if (serial is null) return product;
        return product with
        {
            Serial = new Serial(
                serial.Brand,
                serial.Model,
                serial.Lot,
                serial.ProductionDate)
        };
    }
}