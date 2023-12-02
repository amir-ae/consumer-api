using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Requests;
using Consumer.API.Contract.V1.Products.Responses;
using ErrorOr;

namespace Consumer.API.Client.Resources.Interfaces;

public interface IProductResource
{
    Task<ErrorOr<PaginatedList<ProductResponse>>> ByPage(int? pageSize, int? pageIndex, bool? nextPage = null, 
        string? keyId = null, CancellationToken ct = default);
    Task<ErrorOr<PaginatedList<ProductResponse>>> ByPageDetail(int? pageSize, int? pageIndex, bool? nextPage = null, 
        string? keyId = null, CancellationToken ct = default);
    Task<ErrorOr<IList<ProductForListingResponse>>> All(CancellationToken ct = default);
    Task<ErrorOr<IList<ProductResponse>>> AllDetail(CancellationToken ct = default);
    Task<ErrorOr<IList<ProductResponse>>> DetailByCentreId(Guid centreId, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> DetailByOrderId(string orderId, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> ById(string productId, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> DetailById(string productId, CancellationToken ct = default);
    Task<ErrorOr<ProductEventsResponse>> EventsById(string productId, CancellationToken ct = default);
    Task<ErrorOr<bool>> CheckById(string productId, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Post(PostProductRequest request, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Patch(string productId, PatchProductRequest request, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Activate(string productId, Guid appUserId, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Deactivate(string productId, Guid appUserId, CancellationToken ct = default);
    Task<ErrorOr<ProductResponse>> Delete(string productId, Guid appUserId, CancellationToken ct = default);
}