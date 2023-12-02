namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductUnrepairable(string ProductId,
    bool IsUnrepairable,
    DateTimeOffset? DateOfDemandForCompensation,
    string? DemanderFullName,
    Guid UnrepairableBy,
    DateTimeOffset UnrepairableAt);