namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerPhoneNumberChanged(string CustomerId,
    string PhoneNumber,
    Guid ChangedBy,
    DateTimeOffset ChangedAt);