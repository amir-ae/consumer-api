using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Common.Responses;

public abstract record AuditableResponse
{
    protected AuditableResponse()
    {
    }
    
    [SetsRequiredMembers]
    protected AuditableResponse(
        DateTimeOffset createdAt,
        Guid createdBy,
        DateTimeOffset? lastModifiedAt,
        Guid? lastModifiedBy,
        bool isActive,
        bool isDeleted)
    {
        CreatedAt = createdAt;
        CreatedBy = createdBy;
        LastModifiedAt = lastModifiedAt;
        LastModifiedBy = lastModifiedBy;
        IsActive = isActive;
        IsDeleted = isDeleted;
    }
    
    public required DateTimeOffset CreatedAt { get; init; }
    public required Guid CreatedBy { get; init; }
    public required DateTimeOffset? LastModifiedAt { get; init; }
    public required Guid? LastModifiedBy { get; init; }
    public required bool IsActive { get; init; }
    public required bool IsDeleted { get; init; }
}