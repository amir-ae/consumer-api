using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.Activate;

public record ActivateCustomerCommand(
    AppUserId AppUserId,
    CustomerId CustomerId) : IRequest<ErrorOr<CustomerResponse>>;