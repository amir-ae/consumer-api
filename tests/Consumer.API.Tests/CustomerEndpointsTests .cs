using System.Net;
using System.Net.Http.Json;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Requests;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Fixtures;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Customers = Consumer.API.Contract.V1.Routes.Customers;
using CustomerRole = Consumer.API.Contract.V1.Common.CustomerRole;

namespace Consumer.API.Tests;

public class CustomerEndpointsTests : IntegrationTest
{
    private readonly ConsumerApplicationFactory<Program> _factory;

    public CustomerEndpointsTests(ConsumerApplicationFactory<Program> factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact, TestPriority(1)]
    public async Task get_should_return_success()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Customers.Prefix);

        response.EnsureSuccessStatusCode();
    }
    
    [Theory, TestPriority(2)]
    [InlineData(2)]
    public async Task get_all_should_return_all_customers(int count)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Customers.List.Uri());
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<CustomerResponse>>(content);
        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
    }
    
    [Theory, TestPriority(3)]
    [InlineData(2)]
    public async Task get_all_detail_should_return_all_customers_detail(int count)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Customers.ListDetail.Uri());
       
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<CustomerResponse>>(content);
        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
        result.Select(c => c.Products?.FirstOrDefault()).All(p => p != null).ShouldBeTrue();
    }
    
    [Theory, TestPriority(4)]
    [InlineData(1, 1)]
    public async Task get_should_return_paginated_customers(int pageNumber, int pageSize)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Customers.ByPage.Uri() + $"?pageNumber={pageNumber}&pageSize={pageSize}");
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<PaginatedList<CustomerResponse>>(content);
        result.ShouldNotBeNull();
        result.PageNumber.ShouldBe(pageNumber);
        result.PageSize.ShouldBe(pageSize);
        result.Data.Count().ShouldBe(pageSize);
    }

    [Theory, TestPriority(5)]
    [LoadData("customer.Id")]
    public async Task get_by_id_should_return_customer(CustomerId id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Customers.ById.Uri(id.Value));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(id.Value);
    }
    
    [Theory, TestPriority(6)]
    [LoadData("customer.Id")]
    public async Task get_detail_by_id_should_return_customer_detail(CustomerId id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Customers.DetailById.Uri(id.Value));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(id.Value);
        result.Products?.FirstOrDefault().ShouldNotBeNull();
    }
    
    [Fact, TestPriority(7)]
    public async Task get_by_id_not_present_should_return_not_found()
    {
        var id = new CustomerId(Guid.NewGuid().ToString());
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Customers.ById.Uri(id.Value));
        
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory, TestPriority(8)]
    [LoadData("customer")]
    public async Task add_should_create_new_customer(Customer customer)
    {
        var client = _factory.CreateClient();
        var request = new CreateCustomerRequest(
            Guid.NewGuid(),
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            customer.PhoneNumber,
            customer.CityId.Value,
            customer.Address);
        
        var response = await client.PostAsJsonAsync(Customers.Create.Uri(), request);
        
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location?.ToString().ShouldStartWith(Customers.Prefix);
    }

    [Theory, TestPriority(9)]
    [LoadData("customer")]
    public async Task patch_should_update_customer(Customer customer)
    {
        var client = _factory.CreateClient();
        var customerId = new CustomerId("3");
        var request = new UpdateCustomerRequest(
            Guid.NewGuid(), customer.FirstName, customer.MiddleName, customer.LastName, customer.PhoneNumber);

        var version = 2;
        var eTag = new EntityTagHeaderValue('\"' + version.ToString() + '\"');
        client.DefaultRequestHeaders.TryAddWithoutValidation(HeaderNames.IfMatch, eTag.ToString());
        var response = await client.PatchAsJsonAsync(Customers.Update.Uri(customerId.Value), request);

        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location?.ToString().ShouldContain(Customers.Prefix);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(customerId.Value);
        result.FirstName.ShouldBe(customer.FirstName);
        result.MiddleName.ShouldBe(customer.MiddleName);
        result.LastName.ShouldBe(customer.LastName);
        result.PhoneNumber.ShouldBe(customer.PhoneNumber);
    }
    
    [Theory, TestPriority(10)]
    [LoadData("customer")]
    public async Task get_events_by_id_should_return_customer_events(Customer customer)
    {
        var client = _factory.CreateClient();
        var postRequest = new CreateCustomerRequest(
            Guid.NewGuid(),
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            customer.PhoneNumber.Substring(5),
            customer.CityId.Value,
            customer.Address);
        var patchRequest = new UpdateCustomerRequest(
            Guid.NewGuid(), role: CustomerRole.Dealer);

        var postResponse = await client.PostAsJsonAsync(Customers.Create.Uri(), postRequest);
        var postResponseContent = await postResponse.Content.ReadAsStringAsync();
        var customerId = JsonConvert.DeserializeObject<CustomerResponse>(postResponseContent)!.Id;
        await client.PatchAsJsonAsync(Customers.Update.Uri(customerId), patchRequest);
        var response = await client.GetAsync(Customers.EventsById.Uri(customerId));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerEventsResponse>(content);
        result.ShouldNotBeNull();
        result.CustomerCreatedEvent.ShouldNotBeNull();
        result.CustomerRoleChangedEvents.Last().Role.ShouldBe(CustomerRole.Dealer);
    }

    [Theory, TestPriority(11)]
    [InlineData("3")]
    public async Task activate_should_update_existing_customer(string id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.PatchAsync(Customers.Activate.Uri(id) + $"?activateBy={Guid.NewGuid()}", null);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeTrue();
    }
    
    [Theory, TestPriority(12)]
    [InlineData("3")]
    public async Task deactivate_should_update_existing_customer(string id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.PatchAsync(Customers.Deactivate.Uri(id) + $"?deactivateBy={Guid.NewGuid()}", null);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeFalse();
    }

    [Theory, TestPriority(13)]
    [InlineData("2")]
    public async Task delete_should_remove_existing_customer(string id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.DeleteAsync(Customers.Delete.Uri(id) + $"?deleteBy={Guid.NewGuid()}");
        
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
