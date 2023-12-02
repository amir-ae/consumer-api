using Consumer.API.Contract.V1.Common;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Responses;

namespace Consumer.API.Contract.V1.Customers.Responses;

public record CustomerResponse(string Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string PhoneNumber,
    City City,
    string Address,
    CustomerRole Role,
    IList<string> ProductIds,
    IList<Order> Orders,
    int Version,
    DateTimeOffset CreatedAt,
    Guid CreatedBy,
    DateTimeOffset? LastModifiedAt,
    Guid? LastModifiedBy,
    bool IsActive,
    bool IsDeleted,
    IList<ProductForListingResponse>? Products) : AuditableResponse(Version,
    CreatedAt, 
    CreatedBy, 
    LastModifiedAt, 
    LastModifiedBy,
    IsActive, 
    IsDeleted);