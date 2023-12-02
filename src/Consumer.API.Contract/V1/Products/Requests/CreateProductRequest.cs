using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Products.Requests;

public record CreateProductRequest : CreateRequest
{
    [SetsRequiredMembers]
    public CreateProductRequest(
        Guid createBy,
        string productId,
        string brand,
        string model,
        int? serialId = null,
        Owner? owner = null,
        Dealer? dealer = null,
        string? deviceType = null,
        string? panelModel = null,
        string? panelSerialNumber = null,
        string? warrantyCardNumber = null,
        DateTimeOffset? dateOfPurchase = null,
        string? invoiceNumber = null,
        decimal? purchasePrice = null,
        IEnumerable<Order>? orders = null,
        bool? isUnrepairable = null,
        DateTimeOffset? dateOfDemandForCompensation = null,
        string? demanderFullName = null,
        DateTimeOffset? createAt = null) : base(
        createBy,
        createAt)
    {
        ProductId = productId;
        Brand = brand;
        Model = model;
        SerialId = serialId;
        Owner = owner;
        Dealer = dealer;
        DeviceType = deviceType;
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        WarrantyCardNumber = warrantyCardNumber;
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        Orders = orders;
        IsUnrepairable = isUnrepairable;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
    }
    
    public required string ProductId { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public int? SerialId { get; init; }
    public Owner? Owner { get; init; }
    public Dealer? Dealer { get; init; }
    public string? DeviceType { get; init; }
    public string? PanelModel { get; init; }
    public string? PanelSerialNumber { get; init; }
    public string? WarrantyCardNumber { get; init; }
    public DateTimeOffset? DateOfPurchase { get; init; }
    public string? InvoiceNumber { get; init; }
    public decimal? PurchasePrice { get; init; }
    public IEnumerable<Order>? Orders { get; init; }
    public bool? IsUnrepairable { get; init; }
    public DateTimeOffset? DateOfDemandForCompensation { get; init; }
    public string? DemanderFullName { get; init; }
}