using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Responses;
using ErrorOr;
using MediatR;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;
using ValueTaskSupplement;

namespace Consumer.Application.Customers.Queries.ByPageDetail;

public sealed class CustomersByPageDetailQueryHandler : IRequestHandler<CustomersByPageDetailQuery, ErrorOr<PaginatedList<CustomerResponse>>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEnrichmentService _enrichmentService;

    public CustomersByPageDetailQueryHandler(ICustomerRepository customerRepository, IEnrichmentService enrichmentService)
    {
        _customerRepository = customerRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<PaginatedList<CustomerResponse>>> Handle(CustomersByPageDetailQuery query, CancellationToken ct = default)
    {
        var (pageSize, pageNumber, nextPage, keyId, centreId) = query;

        var (customers, totalCount) = await _customerRepository.ByPageDetailAsync(pageSize, pageNumber, nextPage, keyId, centreId, ct);

        var tasks = customers.Adapt<List<CustomerResponse>>()
            .Select(customer => _enrichmentService.EnrichCustomerResponse(customer, ct));

        var result = await ValueTaskEx.WhenAll(tasks);

        return new PaginatedList<CustomerResponse>(
            pageNumber, pageSize, totalCount, result);
    }
}