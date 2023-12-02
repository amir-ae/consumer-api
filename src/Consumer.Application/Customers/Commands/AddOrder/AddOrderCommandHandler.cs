using Consumer.API.Contract.V1.Customers.Messages;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.Entities;
using Consumer.Domain.Products.ValueObjects;
using MediatR;
using ErrorOr;

namespace Consumer.Application.Customers.Commands.AddOrder;

public sealed class AddOrderCommandHandler : IRequestHandler<AddOrderCommand, ErrorOr<CustomerOrderResponse>>
{
    private readonly IOrderingService _orderingService;
    private readonly ISender _mediator;
    
    public AddOrderCommandHandler(IOrderingService orderingService, ISender mediator)
    {
        _orderingService = orderingService;
        _mediator = mediator;
    }

    public async Task<ErrorOr<CustomerOrderResponse>> Handle(AddOrderCommand command, CancellationToken ct = default)
    {
        var orderId = new OrderId(Guid.NewGuid().ToString());
        var appUserId = command.Product.AppUserId;
        var (ownerCommand, dealerCommand, customerRole, productCommand, conditionCommand) = command;
        var (centreId, completeness, appearance, malfunction, warranty, estimatedCost, 
            dateOfOrder) = conditionCommand;
        
        var createdAt = dateOfOrder ?? DateTimeOffset.UtcNow;

        productCommand = productCommand with
        {
            Orders = new HashSet<Order> { Order.Create(orderId, centreId) }
        };
        
        CustomerResponse? customer = null;
        if (ownerCommand is not null)
        {
            var owner = await _mediator.Send(ownerCommand, ct);
            productCommand = productCommand with { OwnerId = new CustomerId(owner.Value.CustomerId) };
            if (customerRole != CustomerRole.Dealer)
            {
                customer = owner.Value;
            }
        }
        if (dealerCommand is not null)
        {
            var dealer = await _mediator.Send(dealerCommand, ct);
            productCommand = productCommand with { DealerId = new CustomerId(dealer.Value.CustomerId) };
            if (customer is null)
            {
                customer = dealer.Value;
            }
        }

        if (customer is null)
        {
            return Error.Unexpected(nameof(CustomerRole), $"Not expected {nameof(Customer)} role value: {customerRole}");
        }
        
        var product = (await _mediator.Send(productCommand, ct)).Value;

        var orderCustomerMessage = new OrderCustomerMessage(
            customer.CustomerId,
            customer.FullName,
            customer.Role);
        
        var orderProductMessage = new OrderProductMessage(
            product.ProductId,
            product.Brand,
            product.Model,
            warranty,
            completeness,
            appearance,
            malfunction);
        
        var orderForCreationMessage = new OrderForCreationMessage(
            appUserId.Value,
            orderId.Value,
            centreId.Value,
            orderCustomerMessage,
            orderProductMessage,
            createdAt,
            estimatedCost);
        
        await _orderingService.PublishOrderAsync(orderForCreationMessage, ct);

        return new CustomerOrderResponse(
            orderId.Value,
            (int)OrderStatus.Pending,
            centreId.Value,
            customer,
            product, 
            completeness,
            appearance,
            malfunction,
            warranty,
            estimatedCost,
            createdAt,
            appUserId.Value,
            null,
            null,
            true,
            false);
    }
}