using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Responses;
using ErrorOr;
using MediatR;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Mapster;
using ValueTaskSupplement;

namespace Consumer.Application.Customers.Queries.ByPage;

public sealed class CustomersByPageQueryHandler : IRequestHandler<CustomersByPageQuery, ErrorOr<PaginatedList<CustomerResponse>>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEnrichmentService _enrichmentService;

    public CustomersByPageQueryHandler(ICustomerRepository customerRepository, IEnrichmentService enrichmentService)
    {
        _customerRepository = customerRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<PaginatedList<CustomerResponse>>> Handle(CustomersByPageQuery query, CancellationToken ct = default)
    {
        var (pageSize, pageIndex, nextPage, keyId) = query;

        var (customers, totalCount) = await _customerRepository.ByPageAsync(pageSize, pageIndex, nextPage, keyId, ct);

        var tasks = customers.Adapt<List<CustomerResponse>>()
            .Select(customer => _enrichmentService.EnrichCustomerResponse(customer, ct));

        var result = await ValueTaskEx.WhenAll(tasks);

        return new PaginatedList<CustomerResponse>(
            pageIndex, pageSize, totalCount, result);
    }
}