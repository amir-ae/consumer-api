using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.Entities;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Create;

public record CreateProductCommand(
    AppUserId AppUserId,
    ProductId ProductId,
    string Brand,
    string Model,
    SerialId? SerialId,
    Customer? Owner,
    Customer? Dealer,
    string? DeviceType,
    string? PanelModel,
    string? PanelSerialNumber,
    string? WarrantyCardNumber,
    DateTimeOffset? DateOfPurchase,
    string? InvoiceNumber,
    decimal? PurchasePrice,
    HashSet<Order>? Orders,
    bool? IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    DateTimeOffset? CreatedAt) : IRequest<ErrorOr<ProductResponse>>;