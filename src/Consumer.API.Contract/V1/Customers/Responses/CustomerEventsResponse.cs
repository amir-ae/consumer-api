using System.Diagnostics.CodeAnalysis;
using Consumer.API.Contract.V1.Customers.Responses.Events;

namespace Consumer.API.Contract.V1.Customers.Responses;

public sealed record CustomerEventsResponse
{
    public CustomerEventsResponse()
    {
    }

    [SetsRequiredMembers]
    public CustomerEventsResponse(
        CustomerCreated customerCreatedEvent,
        List<CustomerNameChanged>? customerNameChangedEvents = null,
        List<CustomerAddressChanged>? customerAddressChangedEvents = null,
        List<CustomerPhoneNumberChanged>? customerPhoneNumberChangedEvents = null,
        List<CustomerProductAdded>? customerProductAddedEvents = null,
        List<CustomerProductRemoved>? customerProductRemovedEvents = null,
        List<CustomerRoleChanged>? customerRoleChangedEvents = null,
        List<CustomerActivated>? customerActivatedEvents = null,
        List<CustomerDeactivated>? customerDeactivatedEvents = null,
        List<CustomerDeleted>? customerDeletedEvents = null,
        List<CustomerUndeleted>? customerUndeletedEvents = null)
    {
        CustomerCreatedEvent = customerCreatedEvent;
        CustomerNameChangedEvents = customerNameChangedEvents ?? new();
        CustomerAddressChangedEvents = customerAddressChangedEvents ?? new();
        CustomerPhoneNumberChangedEvents = customerPhoneNumberChangedEvents ?? new();
        CustomerProductAddedEvents = customerProductAddedEvents ?? new();
        CustomerProductRemovedEvents = customerProductRemovedEvents ?? new();
        CustomerRoleChangedEvents = customerRoleChangedEvents ?? new();
        CustomerActivatedEvents = customerActivatedEvents ?? new();
        CustomerDeactivatedEvents = customerDeactivatedEvents ?? new();
        CustomerDeletedEvents = customerDeletedEvents ?? new();
        CustomerUndeletedEvents = customerUndeletedEvents ?? new();
    }
    
    public required CustomerCreated CustomerCreatedEvent { get; init; }
    public required List<CustomerNameChanged> CustomerNameChangedEvents { get; init; }
    public required List<CustomerAddressChanged> CustomerAddressChangedEvents { get; init; }
    public required List<CustomerPhoneNumberChanged> CustomerPhoneNumberChangedEvents { get; init; }
    public required List<CustomerProductAdded> CustomerProductAddedEvents { get; init; }
    public required List<CustomerProductRemoved> CustomerProductRemovedEvents { get; init; }
    public required List<CustomerRoleChanged> CustomerRoleChangedEvents { get; init; }
    public required List<CustomerActivated> CustomerActivatedEvents { get; init; }
    public required List<CustomerDeactivated> CustomerDeactivatedEvents { get; init; }
    public required List<CustomerDeleted> CustomerDeletedEvents { get; init; }
    public required List<CustomerUndeleted> CustomerUndeletedEvents { get; init; }
    
}