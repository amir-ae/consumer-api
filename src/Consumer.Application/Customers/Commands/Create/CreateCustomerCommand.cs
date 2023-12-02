using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Order = Consumer.Domain.Common.Entities.Order;

namespace Consumer.Application.Customers.Commands.Create;

public record CreateCustomerCommand(
    string FirstName,
    string? MiddleName,
    string LastName,
    PhoneNumber PhoneNumber,
    CityId CityId,
    string Address,
    CustomerRole? Role,
    HashSet<UpsertProductCommand>? Products,
    HashSet<Order>? Orders,
    AppUserId CreateBy,
    DateTimeOffset? CreateAt,
    bool SaveChanges = true) : IRequest<ErrorOr<Customer>>;