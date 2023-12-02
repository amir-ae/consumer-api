using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Customers.Commands.AddOrder;

public record CreateProductConditionCommand(
    CentreId CentreId,
    string Completeness,
    string Appearance,
    string Malfunction,
    bool Warranty,
    decimal? EstimatedCost,
    DateTimeOffset? DateOfOrder);