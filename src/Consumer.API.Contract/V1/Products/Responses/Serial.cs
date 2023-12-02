namespace Consumer.API.Contract.V1.Products.Responses;

public record Serial(string Brand,
    string Model,
    string? Lot,
    DateTimeOffset? ProductionDate);