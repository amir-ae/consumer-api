using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.API.Contract.V1.Products.Responses;

namespace Consumer.Application.Common.Interfaces.Services;

public interface IEnrichmentService
{
    ValueTask<CustomerResponse> EnrichCustomerResponse(CustomerResponse customer, CancellationToken ct = default);
    ValueTask<CustomerForListingResponse> EnrichCustomerForListingResponse(CustomerForListingResponse customer, CancellationToken ct = default);
    ValueTask<CustomerEventsResponse> EnrichCustomerEvents(CustomerEventsResponse customerEvents, CancellationToken ct = default);
    ValueTask<ProductResponse> EnrichProductResponse(ProductResponse product, CancellationToken ct = default);
}