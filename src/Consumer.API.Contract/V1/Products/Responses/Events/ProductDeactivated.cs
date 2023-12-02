namespace Consumer.API.Contract.V1.Products.Responses.Events;

public sealed record ProductDeactivated(string ProductId,
    Guid DeactivatedBy,
    DateTimeOffset DeactivatedAt);