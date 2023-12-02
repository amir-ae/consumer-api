namespace Consumer.API.Contract.V1.Products.Responses.Events;

public record ProductOwnerChanged(
    string? OwnerId,
    string? OwnerName,
    DateTimeOffset OwnerChangedAt) : ProductEvent;