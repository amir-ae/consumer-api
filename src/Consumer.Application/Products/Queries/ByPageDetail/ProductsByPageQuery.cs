using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Domain.Products.ValueObjects;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Products.Queries.ByPageDetail;

public record ProductsByPageDetailQuery(
    int PageSize,
    int PageIndex,
    bool? NextPage,
    ProductId? KeyId) : IRequest<ErrorOr<PaginatedList<ProductResponse>>>;