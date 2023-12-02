using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Domain.Customers.ValueObjects;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Customers.Queries.EventsById;

public record CustomerEventsByIdQuery(
    CustomerId CustomerId) : IRequest<ErrorOr<CustomerEventsResponse>>;