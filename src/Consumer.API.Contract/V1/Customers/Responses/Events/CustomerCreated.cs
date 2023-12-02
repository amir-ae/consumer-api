using Consumer.API.Contract.V1.Common;

namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerCreated(string CustomerId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string PhoneNumber,
    City City,
    string Address,
    CustomerRole Role,
    IList<string> ProductIds,
    IList<Order> Orders,
    Guid CreatedBy,
    DateTimeOffset CreatedAt);