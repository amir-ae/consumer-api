using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using MediatR;
using ErrorOr;
using Mapster;

namespace Consumer.Application.Customers.Queries.EventsById;

public sealed class CustomerEventsByIdQueryHandler : IRequestHandler<CustomerEventsByIdQuery, ErrorOr<CustomerEventsResponse>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IEnrichmentService _enrichmentService;

    public CustomerEventsByIdQueryHandler(ICustomerRepository customerRepository, IEnrichmentService enrichmentService)
    {
        _customerRepository = customerRepository;
        _enrichmentService = enrichmentService;
    }

    public async Task<ErrorOr<CustomerEventsResponse>> Handle(CustomerEventsByIdQuery query, CancellationToken ct = default)
    {
        var customerId = query.CustomerId;
        var customerEvents = await _customerRepository.EventsByIdAsync(customerId, ct);

        var result = customerEvents.Adapt<CustomerEventsResponse>();
        
        return await _enrichmentService.EnrichCustomerEvents(result, ct);
    }
}