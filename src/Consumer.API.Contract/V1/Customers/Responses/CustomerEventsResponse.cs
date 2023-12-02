using Consumer.API.Contract.V1.Customers.Responses.Events;

namespace Consumer.API.Contract.V1.Customers.Responses;

public sealed record CustomerEventsResponse(CustomerCreated CustomerCreatedEvent,
    IList<CustomerNameChanged> CustomerNameChangedEvents,
    IList<CustomerAddressChanged> CustomerAddressChangedEvents,
    IList<CustomerPhoneNumberChanged> CustomerPhoneNumberChangedEvents,
    IList<CustomerProductAdded> CustomerProductAddedEvents,
    IList<CustomerProductRemoved> CustomerProductRemovedEvents,
    IList<CustomerRoleChanged> CustomerRoleChangedEvents,
    IList<CustomerActivated> CustomerActivatedEvents,
    IList<CustomerDeactivated> CustomerDeactivatedEvents,
    IList<CustomerDeleted> CustomerDeletedEvents,
    IList<CustomerUndeleted> CustomerUndeletedEvents);