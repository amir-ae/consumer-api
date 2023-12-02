using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;
using ValueTaskSupplement;

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
        
        var tasks = customers.Adapt<List<CustomerResponse>>()
            .Select(customer => _enrichmentService.EnrichCustomerResponse(customer, ct));
        
        return await ValueTaskEx.WhenAll(tasks);
    }
}