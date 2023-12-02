using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Common.Requests;

public abstract record PatchRequest
{
    protected PatchRequest()
    {
    }
    
    [SetsRequiredMembers]
    protected PatchRequest(
        Guid patchBy,
        DateTimeOffset? patchAt)
    {
        PatchBy = patchBy;
        PatchAt = patchAt;
    }
    
    public required Guid PatchBy { get; init; }
    public DateTimeOffset? PatchAt { get; init; }
}
