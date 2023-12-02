using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Consumer.API.Contract.V1.Products.Responses;

public sealed record ProductForListingResponse
{
    public ProductForListingResponse()
    {
    }

    [SetsRequiredMembers]
    public ProductForListingResponse(
        string productId,
        string brand,
        string model,
        int? serialId,
        string? ownerId,
        string? dealerId,
        List<ProductOrder> orders,
        DateTimeOffset createdAt,
        string? deviceType = null,
        string? panelModel = null,
        string? panelSerialNumber = null,
        string? warrantyCardNumber = null,
        DateTimeOffset? dateOfPurchase = null,
        string? invoiceNumber = null,
        decimal? purchasePrice = null,
        bool? isUnrepairable = null,
        DateTimeOffset? dateOfDemandForCompensation = null,
        string? demanderFullName = null)
    {
        ProductId = productId;
        Brand = brand;
        Model = model;
        SerialId = serialId;
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
        CreatedAt = createdAt;
    }
    
    public required string ProductId { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public int? SerialId { get; set; }
    public string? OwnerId { get; init; }
    public string? DealerId { get; init; }
    public required List<ProductOrder> Orders { get; init; }
    public string? DeviceType { get; init; }
    public string? PanelModel { get; init; }
    public string? PanelSerialNumber { get; init; }
    public string? WarrantyCardNumber { get; init; }
    public DateTimeOffset? DateOfPurchase { get; init; }
    public string? InvoiceNumber { get; init; }
    public decimal? PurchasePrice { get; init; }
    public bool? IsUnrepairable { get; set; }
    public DateTimeOffset? DateOfDemandForCompensation { get; set; }
    public string? DemanderFullName { get; set; }
    public required DateTimeOffset CreatedAt { get; init; }
}