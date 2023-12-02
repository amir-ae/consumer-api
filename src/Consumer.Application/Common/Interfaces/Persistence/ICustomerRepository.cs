using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Common.Interfaces.Persistence;

public interface ICustomerRepository
{
    Task<bool> CheckByIdAsync(CustomerId id, CancellationToken ct = default);
    Task<Customer?> ByIdAsync(CustomerId id, CancellationToken ct = default);
    Task<Customer?> ByDataAsync(string fullName, string phoneNumber, CancellationToken ct = default);
    Task<Customer?> DetailByIdAsync(CustomerId customerId, CancellationToken ct = default);
    Task<List<Customer>> DetailByIdsAsync(List<CustomerId> ids, CancellationToken ct = default);
    Task<CustomerEvents> EventsByIdAsync(CustomerId id, CancellationToken ct = default);
    Task<(List<Customer>, long)> ByPageAsync(int pageSize, int pageIndex, bool? nextPage, CustomerId? keyId, CancellationToken ct = default);
    Task<(List<Customer>, long)> ByPageDetailAsync(int pageSize, int pageIndex, bool? nextPage, CustomerId? keyId, CancellationToken ct = default);
    Task<List<Customer>> AllAsync(CancellationToken ct = default);
    Task<List<Customer>> AllDetailAsync(CancellationToken ct = default);
    Task<Customer> CreateAsync(CustomerCreatedEvent customerCreatedEvent, CancellationToken ct = default);
    Task<Customer> ActivateAsync(CustomerActivatedEvent customerActivatedEvent, CancellationToken ct = default);
    Task<Customer> DeactivateAsync(CustomerDeactivatedEvent customerDeactivatedEvent, CancellationToken ct = default);
    Task<Customer> UpdateAsync(CustomerEvent customerEvent, Customer? customer = null, CancellationToken ct = default);
    Task<Customer> DeleteAsync(CustomerDeletedEvent customerDeletedEvent, CancellationToken ct = default);
    Task<Customer> UndeleteAsync(CustomerUndeletedEvent customerUndeletedEvent, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}