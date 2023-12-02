using Consumer.API.Client.Base;
using Consumer.API.Client.Resources.Interfaces;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Requests;
using Consumer.API.Contract.V1.Customers.Responses;
using Customers = Consumer.API.Contract.V1.Routes.Customers;
using ErrorOr;

namespace Consumer.API.Client.Resources;

public class CustomerResource : ICustomerResource
{
    private readonly IBaseClient _client;

    public CustomerResource(IBaseClient client)
    {
        _client = client;
    }

    public async Task<ErrorOr<PaginatedList<CustomerResponse>>> ByPage(int? pageSize, int? pageIndex, 
        bool? nextPage = null, string? keyId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (pageSize.HasValue) queries.Add(nameof(pageSize), pageSize.Value.ToString());
        if (pageIndex.HasValue) queries.Add(nameof(pageIndex), pageIndex.Value.ToString());
        if (nextPage.HasValue) queries.Add(nameof(nextPage), nextPage.Value.ToString());
        if (!string.IsNullOrWhiteSpace(keyId)) queries.Add(nameof(keyId), keyId);

        var uri = _client.BuildUri(Customers.ByPage.Uri(), queries);
        return await _client.Get<PaginatedList<CustomerResponse>>(uri, ct);
    }

    public async Task<ErrorOr<PaginatedList<CustomerResponse>>> ByPageDetail(int? pageSize, int? pageIndex, 
        bool? nextPage = null, string? keyId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (pageSize.HasValue) queries.Add(nameof(pageSize), pageSize.Value.ToString());
        if (pageIndex.HasValue) queries.Add(nameof(pageIndex), pageIndex.Value.ToString());
        if (nextPage.HasValue) queries.Add(nameof(nextPage), nextPage.Value.ToString());
        if (!string.IsNullOrWhiteSpace(keyId)) queries.Add(nameof(keyId), keyId);

        var uri = _client.BuildUri(Customers.ByPageDetail.Uri(), queries);
        return await _client.Get<PaginatedList<CustomerResponse>>(uri, ct);
    }

    public async Task<ErrorOr<IList<CustomerForListingResponse>>> All(CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.All.Uri());
        return await _client.Get<IList<CustomerForListingResponse>>(uri, ct);
    }
    
    public async Task<ErrorOr<IList<CustomerResponse>>> AllDetail(CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.AllDetail.Uri());
        return await _client.Get<IList<CustomerResponse>>(uri, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> ById(string customerId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.ById.Uri(customerId));
        return await _client.Get<CustomerResponse>(uri, ct);
    }
    
    public async Task<ErrorOr<CustomerResponse>> DetailById(string customerId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.DetailById.Uri(customerId));
        return await _client.Get<CustomerResponse>(uri, ct);
    }
    
    public async Task<ErrorOr<CustomerEventsResponse>> EventsById(string customerId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.EventsById.Uri(customerId));
        return await _client.Get<CustomerEventsResponse>(uri, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> Post(PostCustomerRequest request, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.Post.Uri());
        return await _client.PostAsJson<CustomerResponse, PostCustomerRequest>(uri, request, ct);
    }
    
    public async Task<ErrorOr<CustomerResponse>> Patch(string customerId, PatchCustomerRequest request, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Customers.Patch.Uri(customerId));
        return await _client.PatchAsJson<CustomerResponse, PatchCustomerRequest>(uri, request, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> Activate(string customerId, Guid appUserId, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(appUserId), appUserId.ToString() } };
        var uri = _client.BuildUri(Customers.Activate.Uri(customerId), queries);
        return await _client.Patch<CustomerResponse>(uri, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> Deactivate(string customerId, Guid appUserId, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(appUserId), appUserId.ToString() } };
        var uri = _client.BuildUri(Customers.Deactivate.Uri(customerId), queries);
        return await _client.Patch<CustomerResponse>(uri, ct);
    }

    public async Task<ErrorOr<CustomerResponse>> Delete(string customerId, Guid appUserId, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(appUserId), appUserId.ToString() } };
        var uri = _client.BuildUri(Customers.Delete.Uri(customerId), queries);
        return await _client.Delete<CustomerResponse>(uri, ct);
    }
}