using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Domain.Products.ValueObjects;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Products.Queries.EventsById;

public record ProductEventsByIdQuery(
    ProductId ProductId) : IRequest<ErrorOr<ProductEventsResponse>>;