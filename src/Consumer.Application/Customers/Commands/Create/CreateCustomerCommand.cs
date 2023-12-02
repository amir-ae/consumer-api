using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Create;

public record CreateCustomerCommand(
    AppUserId AppUserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string PhoneNumber,
    CityId CityId,
    string Address,
    CustomerRole? Role,
    HashSet<Product>? Products,
    DateTimeOffset? CreatedAt) : IRequest<ErrorOr<CustomerResponse>>;