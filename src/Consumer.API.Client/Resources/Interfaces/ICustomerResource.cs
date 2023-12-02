using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Requests;
using Consumer.API.Contract.V1.Customers.Responses;
using ErrorOr;

namespace Consumer.API.Client.Resources.Interfaces;

public interface ICustomerResource
{
    Task<ErrorOr<PaginatedList<CustomerResponse>>> ByPage(int? pageSize, int? pageIndex, bool? nextPage = null, 
        string? keyId = null, CancellationToken ct = default);
    Task<ErrorOr<PaginatedList<CustomerResponse>>> ByPageDetail(int? pageSize, int? pageIndex, bool? nextPage = null, 
        string? keyId = null, CancellationToken ct = default);
    Task<ErrorOr<List<CustomerForListingResponse>>> All(CancellationToken ct = default);
    Task<ErrorOr<List<CustomerResponse>>> AllDetail(CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> ById(string customerId, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> DetailById(string customerId, CancellationToken ct = default);
    Task<ErrorOr<CustomerEventsResponse>> EventsById(string customerId, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Post(PostCustomerRequest request, CancellationToken ct = default);
    Task<ErrorOr<CustomerOrderResponse>> PostOrder(PostCustomerOrderRequest request, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Patch(string customerId, PatchCustomerRequest request, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Activate(string customerId, Guid appUserId, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Deactivate(string customerId, Guid appUserId, CancellationToken ct = default);
    Task<ErrorOr<CustomerResponse>> Delete(string customerId, Guid appUserId, CancellationToken ct = default);
}