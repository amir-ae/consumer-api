using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Queries.DetailByCentreId;

public record ProductsDetailByCentreIdQuery(
    CentreId CentreId) : IRequest<ErrorOr<List<ProductResponse>>>;