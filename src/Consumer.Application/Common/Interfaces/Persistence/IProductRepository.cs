using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Common.Interfaces.Persistence;

public interface IProductRepository
{
    Task<bool> CheckByIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> ByIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> DetailByIdAsync(ProductId id, CancellationToken ct = default);
    Task<ProductEvents> EventsByIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> DetailByOrderIdAsync(OrderId id, CancellationToken ct = default);
    Task<List<Product>> DetailByCentreIdAsync(CentreId centreId, CancellationToken ct = default);
    Task<(List<Product>, long)> ByPageAsync(int pageSize, int pageIndex, bool? nextPage, ProductId? keyId, CancellationToken ct = default);
    Task<(List<Product>, long)> ByPageDetailAsync(int pageSize, int pageIndex, bool? nextPage, ProductId? keyId, CancellationToken ct = default);
    Task<List<Product>> AllAsync(CancellationToken ct = default);
    Task<List<Product>> AllDetailAsync(CancellationToken ct = default);
    Task<Product> CreateAsync(ProductCreatedEvent productCreatedEvent, CancellationToken ct = default);
    Task<Product> UpdateAsync(ProductEvent productEvent, Product? product = null, CancellationToken ct = default);
    Task<Product> ActivateAsync(ProductActivatedEvent productActivatedEvent, CancellationToken ct = default);
    Task<Product> DeactivateAsync(ProductDeactivatedEvent productDeactivatedEvent, CancellationToken ct = default);
    Task<Product> DeleteAsync(ProductDeletedEvent productDeletedEvent, CancellationToken ct = default);
    Task<Product> UndeleteAsync(ProductUndeletedEvent productUndeletedEvent, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}