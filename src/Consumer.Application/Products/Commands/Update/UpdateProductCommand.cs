using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.Entities;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Update;

public record UpdateProductCommand(
    AppUserId AppUserId,
    ProductId ProductId,
    string? Brand,
    string? Model,
    CustomerId? OwnerId,
    CustomerId? DealerId,
    HashSet<Order>? Orders,
    string? DeviceType,
    string? PanelModel,
    string? PanelSerialNumber,
    string? WarrantyCardNumber,
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber,
    decimal? PurchasePrice,
    bool? IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    DateTimeOffset? UpdatedAt,
    bool OnCreate = false,
    bool OnChangingRole = false) : IRequest<ErrorOr<ProductResponse>>;