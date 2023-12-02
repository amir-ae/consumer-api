using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Products.Requests;

public sealed record PatchProductRequest : PatchRequest
{
    public PatchProductRequest()
    {
    }
    
    [SetsRequiredMembers]
    public PatchProductRequest(
        Guid patchBy,
        string? brand = null,
        string? model = null,
        string? ownerId = null,
        string? dealerId = null,
        List<(string, Guid)>? orders = null,
        string? deviceType = null,
        string? panelModel = null,
        string? panelSerialNumber = null,
        string? warrantyCardNumber = null,
        DateTimeOffset? dateOfPurchase = null,
        string? invoiceNumber = null,
        decimal? purchasePrice = null,
        bool? isUnrepairable = null,
        DateTimeOffset? dateOfDemandForCompensation = null,
        string? demanderFullName = null,
        DateTimeOffset? patchAt = null) : base(
        patchBy,
        patchAt)
    {
        Brand = brand;
        Model = model;
        OwnerId = ownerId;
        DealerId = dealerId;
        Orders = orders;
        DeviceType = deviceType;
        PanelModel = panelModel;
        PanelSerialNumber = panelSerialNumber;
        WarrantyCardNumber = warrantyCardNumber;
        DateOfPurchase = dateOfPurchase;
        InvoiceNumber = invoiceNumber;
        PurchasePrice = purchasePrice;
        IsUnrepairable = isUnrepairable;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
    }
    
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? OwnerId { get; init; }
    public string? DealerId { get; init; }
    public List<(string, Guid)>? Orders { get; init; }
    public string? DeviceType { get; init; }
    public string? PanelModel { get; init; }
    public string? PanelSerialNumber { get; init; }
    public string? WarrantyCardNumber { get; init; }
    public DateTimeOffset? DateOfPurchase { get; init; }
    public string? InvoiceNumber { get; set; }
    public decimal? PurchasePrice { get; set; }
    public bool? IsUnrepairable { get; set; }
    public DateTimeOffset? DateOfDemandForCompensation { get; set; }
    public string? DemanderFullName { get; set; }
}