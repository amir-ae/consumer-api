using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Responses;
using ErrorOr;
using MediatR;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;

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
        var (pageSize, pageIndex, nextPage, keyId) = query;

        var (customers, totalCount) = await _customerRepository.ByPageDetailAsync(pageSize, pageIndex, nextPage, keyId, ct);

        var tasks = customers.Adapt<List<CustomerResponse>>()
            .Select(async customer => await _enrichmentService.EnrichCustomerResponse(customer, ct));

        var result = await Task.WhenAll(tasks);

        return new PaginatedList<CustomerResponse>(
            pageIndex, pageSize, totalCount, result);
    }
}