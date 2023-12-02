using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Queries.DetailByOrderId;

public record ProductDetailByOrderIdQuery(
    OrderId OrderId) : IRequest<ErrorOr<ProductResponse>>;