using System.Collections.Concurrent;
using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;

namespace Consumer.Application.Customers.Queries.ListDetail;

public sealed class ListCustomersDetailQueryHandler : IRequestHandler<ListCustomersDetailQuery, ErrorOr<CustomerResponse[]>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEnrichmentService _enrichmentService;

    public ListCustomersDetailQueryHandler(ICustomerRepository customerRepository, IEnrichmentService enrichmentService)
    {
        _customerRepository = customerRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<CustomerResponse[]>> Handle(ListCustomersDetailQuery query, CancellationToken ct = default)
    {
        var centreId = query.CentreId;
        
        var customers = await _customerRepository.ListDetailAsync(centreId, ct);
        
        var customerResponses = new ConcurrentBag<CustomerResponse>();

        await Parallel.ForEachAsync(customers, ct, async (customer, token) =>
        {
            var customerResponse = customer.Adapt<CustomerResponse>();
            customerResponses.Add(await _enrichmentService.EnrichCustomerResponse(customerResponse, token));
        });
        
        return customerResponses
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.LastModifiedAt)
            .ToArray();
    }
}