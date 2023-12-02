using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Common.Requests;

namespace Consumer.API.Contract.V1.Customers.Requests;

public sealed record PostCustomerRequest : PostRequest
{
    [SetsRequiredMembers]
    public PostCustomerRequest(
        Guid postBy,
        string firstName,
        string? middleName,
        string lastName,
        string phoneNumber,
        int cityId,
        string address,
        int? role = null,
        IEnumerable<CustomerProduct>? products = null,
        DateTimeOffset? postAt = null) : base(
        postBy,
        postAt)
    {
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        CityId = cityId;
        Address = address;
        Role = role;
        Products = products;
    }
    
    public required string FirstName { get; init; }
    public string? MiddleName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public required int CityId { get; init; }
    public required string Address { get; init; }
    public int? Role { get; init; }
    public IEnumerable<CustomerProduct>? Products { get; init; }
}

public sealed record CustomerProduct
{
    [SetsRequiredMembers]
    public CustomerProduct(
        string productId,
        string? brand = null,
        string? model = null,
        int? serialId = null,
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
        Orders = orders;
        IsUnrepairable = isUnrepairable;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
    }
    
    public required string ProductId { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public int? SerialId { get; init; }
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