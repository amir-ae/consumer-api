using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Customers.Commands.Update;

public record UpdateCustomerCommand(
    AppUserId AppUserId,
    CustomerId CustomerId,
    string? FirstName,
    string? MiddleName,
    string? LastName,
    string? PhoneNumber,
    CityId? CityId,
    string? Address,
    HashSet<ProductId>? ProductIds,
    CustomerRole? Role,
    DateTimeOffset? UpdatedAt,
    bool OnCreate = false) : IRequest<ErrorOr<CustomerResponse>>;