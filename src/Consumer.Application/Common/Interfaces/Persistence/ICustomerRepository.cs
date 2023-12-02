using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Common.Interfaces.Persistence;

public interface ICustomerRepository
{
    Task<bool> CheckByIdAsync(CustomerId id, CancellationToken ct = default);
    Task<Customer?> ByIdAsync(CustomerId id, CancellationToken ct = default);
    Task<Customer?> ByDataAsync(string fullName, string phoneNumber, CancellationToken ct = default);
    Task<Customer?> DetailByIdAsync(CustomerId customerId, CancellationToken ct = default);
    Task<List<Customer>> DetailByIdsAsync(List<CustomerId> ids, CancellationToken ct = default);
    Task<CustomerEvents?> EventsByIdAsync(CustomerId id, CancellationToken ct = default);
    Task<(List<Customer>, long)> ByPageAsync(int pageSize, int pageIndex, bool? nextPage, CustomerId? keyId, 
        CentreId? centreId, CancellationToken ct = default);
    Task<(List<Customer>, long)> ByPageDetailAsync(int pageSize, int pageIndex, bool? nextPage, CustomerId? keyId, 
        CentreId? centreId, CancellationToken ct = default);
    Task<List<Customer>> ListAsync(CentreId? centreId = null, CancellationToken ct = default);
    Task<List<Customer>> ListDetailAsync(CentreId? centreId = null, CancellationToken ct = default);
    Task<Customer> CreateAsync(CustomerCreatedEvent customerCreatedEvent, CancellationToken ct = default);
    void Append(CustomerEvent customerEvent, int? version = null);
    Task SaveChangesAsync(CancellationToken ct = default);
}