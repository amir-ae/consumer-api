namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public sealed record CustomerPhoneNumberChanged(string CustomerId,
    string PhoneNumber,
    Guid ChangedBy,
    DateTimeOffset ChangedAt);