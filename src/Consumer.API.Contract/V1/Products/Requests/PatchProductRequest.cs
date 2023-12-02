using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Products.Requests;

public sealed record PatchProductRequest : PatchRequest
{
    [SetsRequiredMembers]
    public PatchProductRequest(
        Guid patchBy,
        string? brand = null,
        string? model = null,
        Owner? owner = null,
        Dealer? dealer = null,
        string? deviceType = null,
        string? panelModel = null,
        string? panelSerialNumber = null,
        string? warrantyCardNumber = null,
        DateTimeOffset? dateOfPurchase = null,
        string? invoiceNumber = null,
        decimal? purchasePrice = null,
        IEnumerable<(string, Guid)>? orders = null,
        bool? isUnrepairable = null,
        DateTimeOffset? dateOfDemandForCompensation = null,
        string? demanderFullName = null,
        DateTimeOffset? patchAt = null) : base(
        patchBy,
        patchAt)
    {
        Brand = brand;
        Model = model;
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
    
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public Owner? Owner { get; init; }
    public Dealer? Dealer { get; init; }
    public string? DeviceType { get; init; }
    public string? PanelModel { get; init; }
    public string? PanelSerialNumber { get; init; }
    public string? WarrantyCardNumber { get; init; }
    public DateTimeOffset? DateOfPurchase { get; init; }
    public string? InvoiceNumber { get; init; }
    public decimal? PurchasePrice { get; init; }
    public IEnumerable<(string, Guid)>? Orders { get; init; }
    public bool? IsUnrepairable { get; init; }
    public DateTimeOffset? DateOfDemandForCompensation { get; init; }
    public string? DemanderFullName { get; init; }
}