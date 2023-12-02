using Consumer.API.Client.Base;
using Consumer.API.Client.Resources.Interfaces;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Requests;
using Consumer.API.Contract.V1.Products.Responses;
using Products = Consumer.API.Contract.V1.Routes.Products;
using ErrorOr;

namespace Consumer.API.Client.Resources;

public class ProductResource : IProductResource
{
    private readonly IBaseClient _client;

    public ProductResource(IBaseClient client)
    {
        _client = client;
    }

    public async Task<ErrorOr<PaginatedList<ProductResponse>>> ByPage(int? pageSize, int? pageIndex, 
        bool? nextPage = null, string? keyId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (pageSize.HasValue) queries.Add(nameof(pageSize), pageSize.Value.ToString());
        if (pageIndex.HasValue) queries.Add(nameof(pageIndex), pageIndex.Value.ToString());
        if (nextPage.HasValue) queries.Add(nameof(nextPage), nextPage.Value.ToString());
        if (!string.IsNullOrWhiteSpace(keyId)) queries.Add(nameof(keyId), keyId);

        var uri = _client.BuildUri(Products.ByPage.Uri(), queries);
        return await _client.Get<PaginatedList<ProductResponse>>(uri, ct);
    }

    public async Task<ErrorOr<PaginatedList<ProductResponse>>> ByPageDetail(int? pageSize, int? pageIndex, 
        bool? nextPage = null, string? keyId = null, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string>();
        if (pageSize.HasValue) queries.Add(nameof(pageSize), pageSize.Value.ToString());
        if (pageIndex.HasValue) queries.Add(nameof(pageIndex), pageIndex.Value.ToString());
        if (nextPage.HasValue) queries.Add(nameof(nextPage), nextPage.Value.ToString());
        if (!string.IsNullOrWhiteSpace(keyId)) queries.Add(nameof(keyId), keyId);

        var uri = _client.BuildUri(Products.ByPageDetail.Uri(), queries);
        return await _client.Get<PaginatedList<ProductResponse>>(uri, ct);
    }

    public async Task<ErrorOr<List<ProductForListingResponse>>> All(CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.All.Uri());
        return await _client.Get<List<ProductForListingResponse>>(uri, ct);
    }
    
    public async Task<ErrorOr<List<ProductResponse>>> AllDetail(CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.AllDetail.Uri());
        return await _client.Get<List<ProductResponse>>(uri, ct);
    }
    
    public async Task<ErrorOr<List<ProductResponse>>> DetailByCentreId(Guid centreId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.DetailByCentreId.Uri(centreId));
        return await _client.Get<List<ProductResponse>>(uri, ct);
    }
    
    public async Task<ErrorOr<ProductResponse>> DetailByOrderId(string orderId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.DetailByOrderId.Uri(orderId));
        return await _client.Get<ProductResponse>(uri, ct);
    }
    
    public async Task<ErrorOr<ProductResponse>> ById(string productId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.ById.Uri(productId));
        return await _client.Get<ProductResponse>(uri, ct);
    }
    
    public async Task<ErrorOr<ProductResponse>> DetailById(string productId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.DetailById.Uri(productId));
        return await _client.Get<ProductResponse>(uri, ct);
    }
    
    public async Task<ErrorOr<ProductEventsResponse>> EventsById(string productId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.EventsById.Uri(productId));
        return await _client.Get<ProductEventsResponse>(uri, ct);
    }

    public async Task<ErrorOr<bool>> CheckById(string productId, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.Check.Uri(productId));
        return await _client.Get<bool>(uri, ct);
    }
    
    public async Task<ErrorOr<ProductResponse>> Post(PostProductRequest request, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.Post.Uri());
        return await _client.PostAsJson<ProductResponse, PostProductRequest>(uri, request, ct);
    }

    public async Task<ErrorOr<ProductResponse>> Patch(string productId, PatchProductRequest request, CancellationToken ct = default)
    {
        var uri = _client.BuildUri(Products.Patch.Uri(productId));
        return await _client.PatchAsJson<ProductResponse, PatchProductRequest>(uri, request, ct);
    }

    public async Task<ErrorOr<ProductResponse>> Activate(string productId, Guid appUserId, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(appUserId), appUserId.ToString() } };
        var uri = _client.BuildUri(Products.Activate.Uri(productId), queries);
        return await _client.Patch<ProductResponse>(uri, ct);
    }

    public async Task<ErrorOr<ProductResponse>> Deactivate(string productId, Guid appUserId, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(appUserId), appUserId.ToString() } };
        var uri = _client.BuildUri(Products.Deactivate.Uri(productId), queries);
        return await _client.Patch<ProductResponse>(uri, ct);
    }

    public async Task<ErrorOr<ProductResponse>> Delete(string productId, Guid appUserId, CancellationToken ct = default)
    {
        var queries = new Dictionary<string, string> { { nameof(appUserId), appUserId.ToString() } };
        var uri = _client.BuildUri(Products.Delete.Uri(productId), queries);
        return await _client.Delete<ProductResponse>(uri, ct);
    }
}