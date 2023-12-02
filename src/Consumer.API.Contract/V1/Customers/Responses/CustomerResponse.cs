using Consumer.API.Contract.V1.Common;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Responses;

namespace Consumer.API.Contract.V1.Customers.Responses;

public record CustomerResponse(string Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    PhoneNumber PhoneNumber,
    City City,
    string Address,
    CustomerRole Role,
    IList<string> ProductIds,
    IList<Order> Orders,
    DateTimeOffset CreatedAt,
    Guid CreatedBy,
    DateTimeOffset? LastModifiedAt,
    Guid? LastModifiedBy,
    int Version,
    bool IsActive,
    bool IsDeleted,
    IList<ProductForListingResponse>? Products) : AuditableResponse(CreatedAt, 
    CreatedBy, 
    LastModifiedAt, 
    LastModifiedBy,
    Version,
    IsActive, 
    IsDeleted);