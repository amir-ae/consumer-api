using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Customers.Queries.AllDetail;

public record AllCustomersDetailQuery() : IRequest<ErrorOr<List<CustomerResponse>>>;