using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Fixtures;
using Consumer.Infrastructure.Persistence.Repositories;
using Shouldly;
using Xunit;

namespace Consumer.Infrastructure.Tests;

public class CustomerRepositoryTests : IntegrationTest
{
    private readonly CustomerRepository _sut;

    public CustomerRepositoryTests(ConsumerApplicationFactory<Program> factory) : base(factory)
    {
        _sut = new CustomerRepository(OpenSession());
    }

    [Fact]
    public async Task should_get_data()
    {
        var result = await _sut.AllAsync();

        result.ShouldNotBeNull();
    }
    
    [Theory]
    [InlineData(2)]
    public async Task should_get_all_records(int count)
    {
        await ResetAllDataAsync();
        
        var result = await _sut.AllAsync();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
    }
    
    [Theory]
    [InlineData(2)]
    public async Task should_get_all_records_in_detail(int count)
    {
        await ResetAllDataAsync();
        
        var result = await _sut.AllDetailAsync();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
        result.All(c => c.Products?.FirstOrDefault() != null).ShouldBeTrue();
    }
    
    [Theory]
    [InlineData("3")]
    public async Task should_not_return_deleted_records(string id)
    {
        var result = await _sut.AllAsync();

        result.Any(x => x.Id == new CustomerId(id)).ShouldBeFalse();
    }
    
    [Theory]
    [InlineData("1", "2", "3")]
    public async Task should_return_records_detail_by_ids(string id1, string id2, string id3)
    {
        var ids = new List<string> { id1, id2, id3 };
        var customerIds = ids.Select(id => new CustomerId(id)).ToList();
        
        var result = await _sut.DetailByIdsAsync(customerIds);

        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
    }

    [Theory]
    [LoadData("customer.Id")]
    public async Task should_return_record_by_id(CustomerId id)
    {
        var result = await _sut.ByIdAsync(id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
    }
    
    [Theory]
    [LoadData("customer.Id")]
    public async Task should_return_record_detail_by_id(CustomerId id)
    {
        var result = await _sut.DetailByIdAsync(id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
        result.Products?.FirstOrDefault().ShouldNotBeNull();
    }

    [Theory]
    [LoadData("customer")]
    public async Task should_return_record_by_data(Customer customer)
    {
        await ResetAllDataAsync();
        var result = await _sut.ByDataAsync(customer.FullName, customer.PhoneNumber);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(customer.Id);
    }
    
    [Fact]
    public async Task should_return_null_with_id_not_present()
    {
        var customerId = new CustomerId(Guid.NewGuid().ToString());
        var result = await _sut.ByIdAsync(customerId);

        result.ShouldBeNull();
    }
    
    [Theory]
    [LoadData("customer.Id")]
    public async Task should_return_true_on_check_by_id(CustomerId id)
    {
        var result = await _sut.CheckByIdAsync(id);
        result.ShouldBeTrue();
    }
    
    [Fact]
    public async Task should_return_false_on_check_by_id_not_present()
    {
        var customerId = new CustomerId(Guid.NewGuid().ToString());
        var result = await _sut.CheckByIdAsync(customerId);
        result.ShouldBeFalse();
    }
    
    [Theory]
    [LoadData("customer")]
    public async Task should_add_new_customer(Customer customer)
    {
        var customerId = new CustomerId(Guid.NewGuid().ToString());
        var customerCreatedEvent = new CustomerCreatedEvent(
            customerId,
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            null,
            customer.PhoneNumber,
            customer.CityId,
            customer.Address,
            null,
            customer.ProductIds,
            new AppUserId(Guid.NewGuid())
        );
        
        await _sut.CreateAsync(customerCreatedEvent);
        var result = await _sut.ByIdAsync(customerId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(customerId);
    }
    
    [Theory]
    [LoadData("customer")]
    public async Task should_update_a_customer(Customer customer)
    {
        var customerId = new CustomerId("3");
        var customerNameChangedEvent = new CustomerNameChangedEvent(
            customerId,
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            null,
            new AppUserId(Guid.NewGuid())
        );
        
        await _sut.UpdateAsync(customerNameChangedEvent);
        var result = await _sut.ByIdAsync(customerId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(customerId);
        result.FirstName.ShouldBe(customer.FirstName);
        result.MiddleName.ShouldBe(customer.MiddleName);
        result.LastName.ShouldBe(customer.LastName);
    }
    
    [Theory]
    [LoadData("customer")]
    public async Task should_return_record_events_by_id(Customer customer)
    {
        var customerId = new CustomerId(Guid.NewGuid().ToString());
        var customerCreatedEvent = new CustomerCreatedEvent(
            customerId,
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            null,
            customer.PhoneNumber,
            customer.CityId,
            customer.Address,
            null,
            customer.ProductIds,
            new AppUserId(Guid.NewGuid())
        );
        var customerRoleChangedEvent = new CustomerRoleChangedEvent(
            customerId,
            CustomerRole.Dealer,
            new AppUserId(Guid.NewGuid())
        );
        
        await _sut.CreateAsync(customerCreatedEvent);
        await _sut.UpdateAsync(customerRoleChangedEvent);
        var result = await _sut.EventsByIdAsync(customerId);

        result.ShouldNotBeNull();
        result.CustomerCreatedEvent.ShouldNotBeNull();
        result.CustomerRoleChangedEvents.Last().Role.ShouldBe(CustomerRole.Dealer);
    }
    
    [Theory]
    [InlineData("3")]
    public async Task should_activate_customer(string id)
    {
        var customerActivatedEvent = new CustomerActivatedEvent(
            new CustomerId(id),
            new AppUserId(Guid.NewGuid())
        );
        
        var result = await _sut.ActivateAsync(customerActivatedEvent);
        
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(true);
    }
    
    [Theory]
    [InlineData("3")]
    public async Task should_deactivate_customer(string id)
    {
        var customerDeactivatedEvent = new CustomerDeactivatedEvent(
            new CustomerId(id),
            new AppUserId(Guid.NewGuid())
        );
        
        var result = await _sut.DeactivateAsync(customerDeactivatedEvent);
        
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(false);
    }
    
    [Theory]
    [InlineData("2")]
    public async Task should_delete_customer(string id)
    {
        var customerDeletedEvent = new CustomerDeletedEvent(
            new CustomerId(id),
            new AppUserId(Guid.NewGuid())
        );
        
        var result = await _sut.DeleteAsync(customerDeletedEvent);
        
        result.ShouldNotBeNull();
        result.IsDeleted.ShouldBe(true);
    }
}