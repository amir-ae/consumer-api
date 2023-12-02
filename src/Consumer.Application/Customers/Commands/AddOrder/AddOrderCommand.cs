using Consumer.API.Contract.V1.Customers.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Application.Products.Commands.Create;
using Consumer.Domain.Customers.ValueObjects;

namespace Consumer.Application.Customers.Commands.AddOrder;

public record AddOrderCommand(
    CreateCustomerCommand? Owner,
    CreateCustomerCommand? Dealer,
    CustomerRole CustomerRole,
    CreateProductCommand Product,
    CreateProductConditionCommand ProductCondition) : IRequest<ErrorOr<CustomerOrderResponse>>;