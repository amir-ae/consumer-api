using Consumer.API.Contract.V1.Customers.Responses;
using ErrorOr;
using MediatR;
using Consumer.Application.Common.Interfaces.Persistence;
using Mapster;

namespace Consumer.Application.Customers.Queries.List;

public sealed class ListCustomersQueryHandler : IRequestHandler<ListCustomersQuery, ErrorOr<CustomerForListingResponse[]>>
{
    private readonly ICustomerRepository _customerRepository;

    public ListCustomersQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ErrorOr<CustomerForListingResponse[]>> Handle(ListCustomersQuery query, CancellationToken ct = default)
    {
        var centreId = query.CentreId;
        
        var customers = await _customerRepository.ListAsync(centreId, ct);

        return customers.Adapt<CustomerForListingResponse[]>();
    }
}