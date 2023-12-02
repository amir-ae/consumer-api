using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Order = Consumer.Domain.Common.Entities.Order;

namespace Consumer.Application.Customers.Commands.Update;

public record UpdateCustomerCommand(
    CustomerId CustomerId,
    string? FirstName,
    string? MiddleName,
    string? LastName,
    string? PhoneNumber,
    CityId? CityId,
    string? Address,
    CustomerRole? Role,
    HashSet<UpsertProductCommand>? Products,
    HashSet<Order>? Orders,
    AppUserId UpdateBy,
    DateTimeOffset? UpdateAt,
    bool OnCreate = false,
    int? Version = null) : IRequest<ErrorOr<Customer>>;