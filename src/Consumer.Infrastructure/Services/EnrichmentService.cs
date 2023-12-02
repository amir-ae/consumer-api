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

    public async Task<CustomerResponse> EnrichCustomerResponse(CustomerResponse customer, CancellationToken ct = default)
    {
        var city = await City(customer.City.CityId, ct);
        return Map(customer, city);
    }

    public async Task<CustomerForListingResponse> EnrichCustomerForListingResponse(CustomerForListingResponse customer, CancellationToken ct = default)
    {
        var city = await City(customer.City.CityId, ct);
        return Map(customer, city);
    }

    public async Task<ProductResponse> EnrichProductResponse(ProductResponse product, CancellationToken ct = default)
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
    
    public async Task<CustomerEventsResponse> EnrichCustomerEvents(CustomerEventsResponse customerEvents, CancellationToken ct = default)
    {
        var enrichCreatedTask = EnrichCustomerCreatedEvent(customerEvents.CustomerCreatedEvent, ct);
        var enrichAddressChangedEvents = EnrichCustomerAddressChangedEvents(customerEvents.CustomerAddressChangedEvents, ct);

        await Task.WhenAll(enrichCreatedTask, enrichAddressChangedEvents);

        return customerEvents with
        {
            CustomerCreatedEvent = await enrichCreatedTask,
            CustomerAddressChangedEvents = await enrichAddressChangedEvents
        };
    }

    public async Task<CustomerCreated> EnrichCustomerCreatedEvent(CustomerCreated customerCreatedEvent, CancellationToken ct = default)
    {
        var city = await City(customerCreatedEvent.City.CityId, ct);
        return Map(customerCreatedEvent, city);
    }
    
    public async Task<List<CustomerAddressChanged>> EnrichCustomerAddressChangedEvents(
        List<CustomerAddressChanged> customerAddressChangedEvents, CancellationToken ct = default)
    {
        return (await Task.WhenAll(
            customerAddressChangedEvents.Select(async @event =>
            {
                var city = await City(@event.City.CityId, ct);
                return Map(@event, city);
            }))).ToList();
    }

    public async Task FetchCatalogResources(CancellationToken ct = default)
    {
        var citiesTask = _client.City.All(ct);
        var serialsTask = _client.Serial.All(ct);

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
        var result = await _client.City.All(ct);
        if (!result.IsError)
        {
            Cities = result.Value;
            //_cache.Add("cities", Cities);
        }
    }
    
    public async Task FetchSerials(CancellationToken ct = default)
    {
        var result = await _client.Serial.All(ct);
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

    private async Task<CityResponse?> City(int cityId, CancellationToken ct = default)
    {
        var city = Cities?.FirstOrDefault(c => c.CityId == cityId);
        if (city is not null) return city;

        //CachedCities();
        //city = Cities?.FirstOrDefault(c => c.CityId == cityId);
        //if (city is not null) return city;
        
        await FetchCities(ct);
        return Cities?.FirstOrDefault(c => c.CityId == cityId);
    }

    private async Task<SerialForListingResponse?> Serial(int serialId, CancellationToken ct = default)
    {
        var serial = Serials?.FirstOrDefault(s => s.SerialId == serialId);
        if (serial is not null) return serial;

        //CachedSerials();
        //serial = Serials?.FirstOrDefault(s => s.SerialId == serialId);
        //if (serial is not null) return serial;
        
        await FetchSerials(ct);
        return Serials?.FirstOrDefault(s => s.SerialId == serialId);
    }
    
    private static CustomerResponse Map(CustomerResponse customer, CityResponse? city)
    {
        if (city is null) return customer;
        return customer with
        {
            City = new CustomerCity(
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
            City = new CustomerCity(
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
            City = new CustomerCity(
                @event.City.CityId,
                city.Name,
                city.Oblast,
                city.Code)
        };
    }

    private static CustomerAddressChanged Map(CustomerAddressChanged @event, CityResponse? city)
    {
        if (city is null) return @event;
        return @event with
        {
            City = new CustomerCity(
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
            Serial = new ProductSerial(
                serial.Brand,
                serial.Model,
                serial.Lot,
                serial.ProductionDate)
        };
    }
}