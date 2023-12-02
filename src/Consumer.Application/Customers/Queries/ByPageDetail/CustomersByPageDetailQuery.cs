using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;
using ErrorOr;
using MediatR;

namespace Consumer.Application.Customers.Queries.ByPageDetail;

public record CustomersByPageDetailQuery(
    int PageSize,
    int PageIndex,
    bool? NextPage,
    CustomerId? KeyId,
    CentreId? CentreId) : IRequest<ErrorOr<PaginatedList<CustomerResponse>>>;