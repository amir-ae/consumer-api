using System.Net;
using System.Net.Http.Json;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Requests;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Customers = Consumer.API.Contract.V1.Routes.Customers;

namespace Consumer.API.Tests;
    
public class CustomerEndpointsTests : IntegrationTest
{
    private readonly ConsumerApplicationFactory<Program> _factory;
    private const string Host = "https://localhost:44302";

    public CustomerEndpointsTests(ConsumerApplicationFactory<Program> factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task get_should_return_success()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Customers.Prefix);

        response.EnsureSuccessStatusCode();
    }
    
    [Theory]
    [InlineData(2)]
    public async Task get_all_should_return_all_customers(int count)
    {
        await ResetAllDataAsync();
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Customers.All.Uri());
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<CustomerResponse>>(content);
        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
    }
    
    [Theory]
    [InlineData(2)]
    public async Task get_all_detail_should_return_all_customers_detail(int count)
    {
        await ResetAllDataAsync();
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Customers.AllDetail.Uri());
       
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<CustomerResponse>>(content);
        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
        result.Select(c => c.Products?.FirstOrDefault()).All(p => p != null).ShouldBeTrue();
    }
    
    [Theory]
    [InlineData(1, 1)]
    public async Task get_should_return_paginated_customers(int pageIndex, int pageSize)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Customers.ByPage.Uri() + $"?pageIndex={pageIndex}&pageSize={pageSize}");
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<PaginatedList<CustomerResponse>>(content);
        result.ShouldNotBeNull();
        result.PageIndex.ShouldBe(pageIndex);
        result.PageSize.ShouldBe(pageSize);
        result.Data.Count().ShouldBe(pageSize);
    }

    [Theory]
    [LoadData("customer.Id")]
    public async Task get_by_id_should_return_customer(CustomerId id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Customers.ById.Uri(id.Value));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(id.Value);
    }
    
    [Theory]
    [LoadData("customer.Id")]
    public async Task get_detail_by_id_should_return_customer_detail(CustomerId id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Customers.DetailById.Uri(id.Value));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(id.Value);
        result.Products?.FirstOrDefault().ShouldNotBeNull();
    }
    
    [Fact]
    public async Task get_by_id_not_present_should_return_not_found()
    {
        var id = new CustomerId(Guid.NewGuid().ToString());
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Customers.ById.Uri(id.Value));
        
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory]
    [LoadData("customer")]
    public async Task add_should_create_new_customer(Customer customer)
    {
        var client = _factory.CreateClient();
        var request = new PostCustomerRequest(
            Guid.NewGuid(),
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            customer.PhoneNumber,
            customer.CityId.Value,
            customer.Address);
        
        var response = await client.PostAsJsonAsync(Customers.Post.Uri(), request);
        
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location?.ToString().ShouldStartWith(Customers.Prefix);
    }

    [Theory]
    [LoadData("customer")]
    public async Task patch_should_update_customer(Customer customer)
    {
        var client = _factory.CreateClient();
        var customerId = new CustomerId("3");
        var request = new PatchCustomerRequest(
            Guid.NewGuid(), customer.FirstName, customer.MiddleName, customer.LastName);

        var response = await client.PatchAsJsonAsync(
            Host + Customers.Patch.Uri(customerId.Value), request);
        
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location?.ToString().ShouldContain(Customers.Prefix);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.CustomerId.ShouldBe(customerId.Value);
        result.FirstName.ShouldBe(customer.FirstName);
        result.MiddleName.ShouldBe(customer.MiddleName);
        result.LastName.ShouldBe(customer.LastName);
    }
    
    [Theory]
    [LoadData("customer")]
    public async Task get_events_by_id_should_return_customer_events(Customer customer)
    {
        await ResetAllDataAsync();
        var client = _factory.CreateClient();
        var postRequest = new PostCustomerRequest(
            Guid.NewGuid(),
            customer.FirstName,
            customer.MiddleName,
            customer.LastName,
            customer.PhoneNumber.Substring(5),
            customer.CityId.Value,
            customer.Address);
        var patchRequest = new PatchCustomerRequest(
            Guid.NewGuid(), role: (int)CustomerRole.Dealer);

        var postResponse = await client.PostAsJsonAsync(Customers.Post.Uri(), postRequest);
        var postResponseContent = await postResponse.Content.ReadAsStringAsync();
        var customerId = JsonConvert.DeserializeObject<CustomerResponse>(postResponseContent)!.CustomerId;
        await client.PatchAsJsonAsync(Host + Customers.Patch.Uri(customerId), patchRequest);
        var response = await client.GetAsync(Customers.EventsById.Uri(customerId));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerEventsResponse>(content);
        result.ShouldNotBeNull();
        result.CustomerCreatedEvent.ShouldNotBeNull();
        result.CustomerRoleChangedEvents.Last().Role.ShouldBe((int)CustomerRole.Dealer);
    }

    [Theory]
    [InlineData("3")]
    public async Task activate_should_update_existing_customer(string id)
    {
        var client = _factory.CreateClient();
        var appUserId = Guid.NewGuid();

        var response = await client.PatchAsync(Customers.Activate.Uri(id) + $"?appUserId={appUserId}", null);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeTrue();
    }
    
    [Theory]
    [InlineData("3")]
    public async Task deactivate_should_update_existing_customer(string id)
    {
        var client = _factory.CreateClient();
        var appUserId = Guid.NewGuid();

        var response = await client.PatchAsync(Customers.Deactivate.Uri(id) + $"?appUserId={appUserId}", null);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CustomerResponse>(content);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeFalse();
    }

    [Theory]
    [InlineData("2")]
    public async Task delete_should_remove_existing_customer(string id)
    {
        var client = _factory.CreateClient();
        var appUserId = Guid.NewGuid();
    
        var response = await client.DeleteAsync(Customers.Delete.Uri(id) + $"?appUserId={appUserId}");
        
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
