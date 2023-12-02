using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Products.Requests;

public sealed record PostProductRequest : PostRequest
{
    [SetsRequiredMembers]
    public PostProductRequest(
        Guid postBy,
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
        IEnumerable<(string, Guid)>? orders = null,
        bool? isUnrepairable = null,
        DateTimeOffset? dateOfDemandForCompensation = null,
        string? demanderFullName = null,
        DateTimeOffset? postAt = null) : base(
        postBy,
        postAt)
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
    public IEnumerable<(string, Guid)>? Orders { get; init; }
    public bool? IsUnrepairable { get; init; }
    public DateTimeOffset? DateOfDemandForCompensation { get; init; }
    public string? DemanderFullName { get; init; }
}

public sealed record Owner
{
    [SetsRequiredMembers]
    public Owner(
        string? ownerId,
        string? firstName = null,
        string? middleName = null,
        string? lastName = null,
        string? phoneNumber = null,
        int? cityId = null,
        string? address = null)
    {
        OwnerId = ownerId;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
    }
    
    public string? OwnerId { get; init; }
    public string? FirstName { get; init; }
    public string? MiddleName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public int? CityId { get; init; }
    public string? Address { get; init; }
}

public sealed record Dealer
{
    [SetsRequiredMembers]
    public Dealer(
        string? dealerId,
        string? name = null,
        string? phoneNumber = null,
        int? cityId = null,
        string? address = null)
    {
        DealerId = dealerId;
        Name = name;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
    }
    
    public string? DealerId { get; init; }
    public string? Name { get; init; }
    public string? PhoneNumber { get; init; }
    public int? CityId { get; init; }
    public string? Address { get; init; }
}