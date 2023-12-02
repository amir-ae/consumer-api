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
        AppUserId unrepairableBy,
        DateTimeOffset? unrepairableAt = null) : base(
        productId)
    {
        IsUnrepairable = isUnrepairable;
        DateOfDemandForCompensation = dateOfDemandForCompensation;
        DemanderFullName = demanderFullName;
        UnrepairableBy = unrepairableBy;
        UnrepairableAt = unrepairableAt ?? DateTimeOffset.UtcNow;
    }
    
    public required bool IsUnrepairable { get; set; }
    public required DateTimeOffset? DateOfDemandForCompensation { get; set; }
    public required string? DemanderFullName { get; set; }
    public required AppUserId UnrepairableBy { get; init; }
    public DateTimeOffset UnrepairableAt { get; init; }
}