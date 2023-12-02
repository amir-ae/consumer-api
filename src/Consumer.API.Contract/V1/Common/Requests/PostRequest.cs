using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Common.Requests;

public abstract record PostRequest
{
    [SetsRequiredMembers]
    protected PostRequest(
        Guid postBy,
        DateTimeOffset? postAt)
    {
        PostBy = postBy;
        PostAt = postAt;
    }
    
    public required Guid PostBy { get; init; }
    public DateTimeOffset? PostAt { get; init; }
}
