using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.API.Contract.V1.Customers.Responses.Events;
using Consumer.API.Contract.V1.Products.Responses;

namespace Consumer.Application.Common.Interfaces.Services;

public interface IEnrichmentService
{
    Task<CustomerResponse> EnrichCustomerResponse(CustomerResponse customer, CancellationToken ct = default);
    Task<CustomerForListingResponse> EnrichCustomerForListingResponse(CustomerForListingResponse customer, CancellationToken ct = default);
    Task<CustomerEventsResponse> EnrichCustomerEvents(CustomerEventsResponse customerEvents, CancellationToken ct = default);
    Task<ProductResponse> EnrichProductResponse(ProductResponse product, CancellationToken ct = default);
    Task FetchCatalogResources(CancellationToken ct = default);
}