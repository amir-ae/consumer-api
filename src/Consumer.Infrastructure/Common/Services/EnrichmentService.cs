using System.Text.Json;
using Catalog.API.Client;
using Catalog.API.Contract.V1.Cities.Responses;
using Catalog.API.Contract.V1.Serials.Responses;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.API.Contract.V1.Customers.Responses.Events;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Consumer.Infrastructure.Common.Services;

public class EnrichmentService : IEnrichmentService
{
    private readonly ICatalogClient _client;
    private readonly IDistributedCache _cache;
    private readonly JsonSerializerOptions _serializerOptions;

    public EnrichmentService(ICatalogClient client, IDistributedCache cache)
    {
        _client = client;
        _cache = cache;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

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

    private async ValueTask<CityResponse?> City(int cityId, CancellationToken ct = default)
    {
        var cacheKey = $"city_{cityId}";

        var cachedCity = await _cache.GetStringAsync(cacheKey, token: ct);
        if (cachedCity is not null)
        {
            return JsonSerializer.Deserialize<CityResponse>(cachedCity, _serializerOptions);
        }
        
        var city = await FetchCity(cityId, ct);

        if (city is not null)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(city, _serializerOptions), cacheOptions, token: ct);
            return city;
        }

        return null;
    }

    private async Task<CityResponse?> FetchCity(int cityId, CancellationToken ct = default)
    {
        try
        {
            var result = await _client.City.ById(cityId, ct);
            if (!result.IsError)
            {
                return result.Value;
            }
        }
        catch
        {
            //ignore
        }
        
        return null;
    }
    
    private async ValueTask<SerialResponse?> Serial(int serialId, CancellationToken ct = default)
    {
        var cacheKey = $"serial_{serialId}";

        var cachedSerial = await _cache.GetStringAsync(cacheKey, token: ct);
        if (cachedSerial is not null)
        {
            return JsonSerializer.Deserialize<SerialResponse>(cachedSerial, _serializerOptions);
        }
        
        var serial = await FetchSerial(serialId, ct);

        if (serial is not null)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(serial, _serializerOptions), cacheOptions, token: ct);
            return serial;
        }
        
        return null;
    }

    private async Task<SerialResponse?> FetchSerial(int serialId, CancellationToken ct = default)
    {
        try
        {
            var result = await _client.Serial.ById(serialId, ct);
            if (!result.IsError)
            {
                return result.Value;
            }
        }
        catch
        {
            //ignore
        }
        
        return null;
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
                city.PostalCode,
                city.PhoneCode)
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
                city.PostalCode,
                city.PhoneCode)
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
                city.PostalCode,
                city.PhoneCode)
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
                city.PostalCode,
                city.PhoneCode)
        };
    }
    
    private static ProductResponse Map(ProductResponse product, SerialResponse? serial)
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