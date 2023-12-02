using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Common.Interfaces.Persistence;

public interface IProductRepository
{
    Task<bool> CheckByIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> ByIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> ByStreamIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> DetailByIdAsync(ProductId id, CancellationToken ct = default);
    Task<ProductEvents?> EventsByIdAsync(ProductId id, CancellationToken ct = default);
    Task<Product?> DetailByOrderIdAsync(OrderId id, CancellationToken ct = default);
    Task<(List<Product>, long)> ByPageAsync(int pageSize, int pageNumber, bool? nextPage, ProductId? keyId, 
        CentreId? centreId, CancellationToken ct = default);
    Task<(List<Product>, long)> ByPageDetailAsync(int pageSize, int pageNumber, bool? nextPage, ProductId? keyId, 
        CentreId? centreId, CancellationToken ct = default);
    Task<List<Product>> ListAsync(CentreId? centreId = null, CancellationToken ct = default);
    Task<List<Product>> ListDetailAsync(CentreId? centreId = null, CancellationToken ct = default);
    Task<Product> CreateAsync(ProductCreatedEvent productCreatedEvent, CancellationToken ct = default);
    void Append(ProductEvent productEvent);
    Task SaveChangesAsync(CancellationToken ct = default);
}