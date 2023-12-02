using Consumer.API.Contract.V1.Customers.Responses;
using ErrorOr;
using MediatR;

namespace Consumer.Application.Customers.Queries.All;

public record AllCustomersQuery() : IRequest<ErrorOr<List<CustomerForListingResponse>>>;