using System.Diagnostics.CodeAnalysis;
using Consumer.Domain.Customers.Events;

namespace Consumer.Domain.Customers;

public sealed record CustomerEvents
{
    [SetsRequiredMembers]
    public CustomerEvents(
        CustomerCreatedEvent customerCreatedEvent,
        List<CustomerNameChangedEvent>? customerNameChangedEvents = null,
        List<CustomerAddressChangedEvent>? customerAddressChangedEvents = null,
        List<CustomerPhoneNumberChangedEvent>? customerPhoneNumberChangedEvents = null,
        List<CustomerProductAddedEvent>? customerProductAddedEvents = null,
        List<CustomerProductRemovedEvent>? customerProductRemovedEvents = null,
        List<CustomerRoleChangedEvent>? customerRoleChangedEvents = null,
        List<CustomerActivatedEvent>? customerActivatedEvents = null,
        List<CustomerDeactivatedEvent>? customerDeactivatedEvents = null,
        List<CustomerDeletedEvent>? customerDeletedEvents = null,
        List<CustomerUndeletedEvent>? customerUndeletedEvents = null)
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
    
    public required CustomerCreatedEvent CustomerCreatedEvent { get; init; }
    public List<CustomerNameChangedEvent> CustomerNameChangedEvents { get; set; }
    public List<CustomerAddressChangedEvent> CustomerAddressChangedEvents { get; set; }
    public List<CustomerPhoneNumberChangedEvent> CustomerPhoneNumberChangedEvents { get; set; }
    public List<CustomerProductAddedEvent> CustomerProductAddedEvents { get; set; }
    public List<CustomerProductRemovedEvent> CustomerProductRemovedEvents { get; set; }
    public List<CustomerRoleChangedEvent> CustomerRoleChangedEvents { get; init; }
    public List<CustomerActivatedEvent> CustomerActivatedEvents { get; set; }
    public List<CustomerDeactivatedEvent> CustomerDeactivatedEvents { get; set; }
    public List<CustomerDeletedEvent> CustomerDeletedEvents { get; set; }
    public List<CustomerUndeletedEvent> CustomerUndeletedEvents { get; set; }
}