using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Domain.Products.Events;

public sealed record ProductUnrepairableEvent : ProductEvent
{
    [SetsRequiredMembers]
    public ProductUnrepairableEvent(
        ProductId productId,
        bool isUnrepairable,
        DateTimeOffset? dateOfDemandForCompensation,
        string? demanderFullName,
        AppUserId actor,
        DateTimeOffset? unrepairableAt = null) : base(
        productId, actor)
    {
        IsUnrepairable = isUnrepairable;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
        UnrepairableAt = unrepairableAt ?? DateTimeOffset.UtcNow;
    }
    
    public required bool IsUnrepairable { get; init; }
    public required DateTimeOffset? DateOfDemandForCompensation { get; init; }
    public required string? DemanderFullName { get; init; }
    public DateTimeOffset UnrepairableAt { get; init; }
}