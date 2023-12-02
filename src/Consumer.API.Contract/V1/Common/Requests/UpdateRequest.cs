using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Common.Requests;

public abstract record UpdateRequest
{
    [SetsRequiredMembers]
    protected UpdateRequest(
        Guid updateBy,
        DateTimeOffset? updateAt)
    {
        UpdateBy = updateBy;
        UpdateAt = updateAt;
    }
    
    public required Guid UpdateBy { get; init; }
    public DateTimeOffset? UpdateAt { get; init; }
}
