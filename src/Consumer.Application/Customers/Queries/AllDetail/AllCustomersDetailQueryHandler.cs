using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;
using ValueTaskSupplement;

namespace Consumer.Application.Customers.Queries.AllDetail;

public sealed class AllCustomersDetailQueryHandler : IRequestHandler<AllCustomersDetailQuery, ErrorOr<CustomerResponse[]>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEnrichmentService _enrichmentService;

    public AllCustomersDetailQueryHandler(ICustomerRepository customerRepository, IEnrichmentService enrichmentService)
    {
        _customerRepository = customerRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<CustomerResponse[]>> Handle(AllCustomersDetailQuery query, CancellationToken ct = default)
    {
        var customers = await _customerRepository.AllDetailAsync(ct);

        var tasks = customers.Adapt<List<CustomerResponse>>()
            .Select(customer => _enrichmentService.EnrichCustomerResponse(customer, ct));
        
        return await ValueTaskEx.WhenAll(tasks);
    }
}