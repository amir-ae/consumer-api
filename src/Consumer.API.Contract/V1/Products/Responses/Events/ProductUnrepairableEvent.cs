using System.Diagnostics.CodeAnalysis;

namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductUnrepairable
{
    public ProductUnrepairable()
    {
    }

    [SetsRequiredMembers]
    public ProductUnrepairable(
        string productId,
        bool isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        Guid unrepairableBy,
        DateTimeOffset? unrepairableAt = null)
    {
        ProductId = productId;
        IsUnrepairable = isUnrepairable;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
        UnrepairableBy = unrepairableBy;
        UnrepairableAt = unrepairableAt ?? DateTimeOffset.UtcNow;
    }
    
    public required string ProductId { get; init; }
    public required bool IsUnrepairable { get; set; }
    public required DateTimeOffset? DateOfDemandForCompensation { get; set; }
    public required string? DemanderFullName { get; set; }
    public required Guid UnrepairableBy { get; init; }
    public required DateTimeOffset UnrepairableAt { get; init; }
}