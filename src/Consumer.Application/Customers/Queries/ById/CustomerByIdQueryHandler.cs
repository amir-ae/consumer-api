using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Domain.Customers;
using Mapster;

namespace Consumer.Application.Customers.Queries.ById;

public sealed class CustomerByIdQueryHandler : IRequestHandler<CustomerByIdQuery, ErrorOr<CustomerResponse>>
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerByIdQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<CustomerResponse>> Handle(CustomerByIdQuery query, CancellationToken ct = default)
    {
        var customer = await _customerRepository.ByIdAsync(query.CustomerId, ct);
            
        if (customer is null) return Error.NotFound(
            nameof(query.CustomerId), $"{nameof(Customer)} with id {query.CustomerId.Value} is not found.");
        
        return customer.Adapt<CustomerResponse>();
    }
}