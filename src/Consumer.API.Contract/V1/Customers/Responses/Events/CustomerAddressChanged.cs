namespace Consumer.API.Contract.V1.Customers.Responses.Events;

public record CustomerAddressChanged(
    City City,
    string Address,
    DateTimeOffset AddressChangedAt) : CustomerEvent;