﻿using System.Collections.Concurrent;
using Commerce.Application.Common.Interfaces.Persistence;
using Commerce.Application.Common.Interfaces.Services;
using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Customers.Responses;
using ErrorOr;
using MediatR;
using Mapster;

namespace Commerce.Application.Customers.Queries.ByPage;

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
        var (pageSize, pageNumber, nextPage, keyId, centreId) = query;

        var (customers, totalCount) = await _customerRepository.ByPageAsync(pageSize, pageNumber, nextPage, keyId, centreId, ct);

        var customerResponses = new ConcurrentBag<CustomerResponse>();

        await Parallel.ForEachAsync(customers, ct, async (customer, token) =>
        {
            var customerResponse = customer.Adapt<CustomerResponse>();
            customerResponses.Add(await _enrichmentService.EnrichCustomerResponse(customerResponse, token));
        });

        return new PaginatedList<CustomerResponse>(
            pageNumber, pageSize, totalCount, customerResponses
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.LastModifiedAt)
                .ToArray());
    }
}