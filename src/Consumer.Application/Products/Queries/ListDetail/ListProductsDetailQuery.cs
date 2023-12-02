using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Domain.Products.ValueObjects;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Products.Queries.ListDetail;

public record ListProductsDetailQuery(
    CentreId? CentreId) : IRequest<ErrorOr<ProductResponse[]>>;