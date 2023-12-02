using Consumer.API.Contract.V1.Customers.Responses;
using ErrorOr;
using MediatR;
using Consumer.Application.Common.Interfaces.Persistence;
using Mapster;

namespace Consumer.Application.Customers.Queries.All;

public sealed class AllCustomersQueryHandler : IRequestHandler<AllCustomersQuery, ErrorOr<List<CustomerForListingResponse>>>
{
    private readonly ICustomerRepository _customerRepository;

    public AllCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<List<CustomerForListingResponse>>> Handle(AllCustomersQuery query, CancellationToken ct = default)
    {
        var customers = await _customerRepository.AllAsync(ct);

        return customers.Adapt<List<CustomerForListingResponse>>();
    }
}