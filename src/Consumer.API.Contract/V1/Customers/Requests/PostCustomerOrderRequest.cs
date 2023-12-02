using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Customers.Requests;

public sealed record PostCustomerOrderRequest : PostRequest
{
    public PostCustomerOrderRequest()
    {
    }

    [SetsRequiredMembers]
    public PostCustomerOrderRequest(
        Guid postBy,
        Guid centreId,
        PostOrderOwner? owner,
        PostOrderDealer? dealer,
        int customerRole,
        PostOrderProduct product,
        PostOrderProductCondition productCondition,
        DateTimeOffset? postAt = null) : base(
        postBy,
        postAt)
    {
        CentreId = centreId;
        Dealer = dealer;
        CustomerRole = customerRole;
        Owner = owner;
        Product = product;
        ProductCondition = productCondition;
    }
    
    public required Guid CentreId { get; init; }
    public required PostOrderOwner? Owner { get; init; }
    public required PostOrderDealer? Dealer { get; init; }
    public required int CustomerRole { get; init; }
    public required PostOrderProduct Product { get; init; }
    public required PostOrderProductCondition ProductCondition { get; init; }
}

public record PostOrderOwner
{
    public PostOrderOwner()
    {
    }

    [SetsRequiredMembers]
    public PostOrderOwner(
        string firstName,
        string? middleName,
        string lastName,
        string phoneNumber,
        int cityId,
        string address)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
    }
    
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public required int CityId { get; init; }
    public required string Address { get; init; }
}

public record PostOrderDealer
{
    public PostOrderDealer()
    {
    }

    [SetsRequiredMembers]
    public PostOrderDealer(
        string name,
        string phoneNumber,
        int cityId,
        string address)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
    }
    
    public required string Name { get; init; }
    public required string PhoneNumber { get; init; }
    public required int CityId { get; init; }
    public required string Address { get; init; }
}

public record PostOrderProduct
{
    public PostOrderProduct()
    {
    }

    [SetsRequiredMembers]
    public PostOrderProduct(
        string productId,
        string brand,
        string model,
        int? serialId,
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
    
    public required string ProductId { get; init; }
    public required string Brand { get; init; }
    public required string Model { get; init; }
    public int? SerialId { get; init; }
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

public record PostOrderProductCondition
{
    public PostOrderProductCondition()
    {
    }

    [SetsRequiredMembers]
    public PostOrderProductCondition(
        string completeness,
        string appearance,
        string malfunction,
        bool warranty = false,
        decimal? estimatedCost = null)
    {
        Completeness = completeness;
        Appearance = appearance;
        Malfunction = malfunction;
        Warranty = warranty;
        EstimatedCost = estimatedCost;
    }
    
    public required string Completeness { get; init; }
    public required string Appearance { get; init; }
    public required string Malfunction { get; init; }
    public bool Warranty { get; init; }
    public decimal? EstimatedCost { get; init; }
}