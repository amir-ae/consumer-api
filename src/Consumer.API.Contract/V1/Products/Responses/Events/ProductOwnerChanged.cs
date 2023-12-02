namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductOwnerChanged(string ProductId,
    string? OwnerId,
    string? OwnerName,
    Guid OwnerChangedBy,
    DateTimeOffset OwnerChangedAt);