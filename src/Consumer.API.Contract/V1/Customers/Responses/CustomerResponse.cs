using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Responses;

namespace Consumer.API.Contract.V1.Customers.Responses;

public sealed record CustomerResponse(string CustomerId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string PhoneNumber,
    CustomerCity City,
    string Address,
    int Role,
    IList<string> ProductIds,
    DateTimeOffset CreatedAt,
    Guid CreatedBy,
    DateTimeOffset? LastModifiedAt,
    Guid? LastModifiedBy,
    bool IsActive,
    bool IsDeleted,
    IList<ProductForListingResponse>? Products) : AuditableResponse(CreatedAt, 
    CreatedBy, 
    LastModifiedAt, 
    LastModifiedBy,
    IsActive, 
    IsDeleted);

public sealed record CustomerCity(int CityId,
    string? Name,
    string? Oblast,
    string? Code);