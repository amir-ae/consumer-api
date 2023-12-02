using System.Net;
using System.Net.Http.Json;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Requests;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;
using Consumer.Fixtures;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Products = Consumer.API.Contract.V1.Routes.Products;

namespace Consumer.API.Tests;

public class ProductEndpointsTests : IntegrationTest
{
    private readonly ConsumerApplicationFactory<Program> _factory;

    public ProductEndpointsTests(ConsumerApplicationFactory<Program> factory) : base(factory)
    {
        _factory = factory;
    }

    [Fact, TestPriority(1)]
    public async Task get_should_return_success()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Products.Prefix);

        response.EnsureSuccessStatusCode();
    }
    
    [Theory, TestPriority(2)]
    [InlineData(2)]
    public async Task get_all_should_return_all_products(int count)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Products.List.Uri());
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<ProductResponse>>(content);
        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
    }
    
    [Theory, TestPriority(3)]
    [InlineData(2)]
    public async Task get_all_detail_should_return_all_products_detail(int count)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Products.ListDetail.Uri());
       
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<List<ProductResponse>>(content);
        result.ShouldNotBeNull();
        result.Count.ShouldBe(count);
        result.Select(p => p.Owner).All(p => p != null).ShouldBeTrue();
    }
    
    [Theory, TestPriority(4)]
    [InlineData(1, 1)]
    public async Task get_should_return_paginated_products(int pageNumber, int pageSize)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(Products.ByPage.Uri() + $"?pageNumber={pageNumber}&pageSize={pageSize}");
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<PaginatedList<ProductResponse>>(content);
        result.ShouldNotBeNull();
        result.PageNumber.ShouldBe(pageNumber);
        result.PageSize.ShouldBe(pageSize);
        result.Data.Count().ShouldBe(pageSize);
    }

    [Theory, TestPriority(5)]
    [LoadData("product.Id")]
    public async Task get_by_id_should_return_product(ProductId id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Products.ById.Uri(id.Value));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProductResponse>(content);
        result.ShouldNotBeNull();
    }
    
    [Theory, TestPriority(6)]
    [LoadData("product.Id")]
    public async Task get_detail_by_id_should_return_product_detail(ProductId id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Products.DetailById.Uri(id.Value));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProductResponse>(content);
        result.ShouldNotBeNull();
        result.Owner.ShouldNotBeNull();
    }

    [Theory, TestPriority(7)]
    [LoadData("product")]
    public async Task get_by_order_id_should_return_product_detail(Product product)
    {
        var orderId = product.Orders.First().Id;
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Products.DetailByOrderId.Uri(orderId.Value));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProductResponse>(content);
        result.ShouldNotBeNull();
        result.Orders.Select(o => o.Id).ShouldContain(orderId.Value);
        result.Owner.ShouldNotBeNull();
    }

    [Fact, TestPriority(8)]
    public async Task get_by_id_not_present_should_return_not_found()
    {
        var id = new ProductId(Guid.NewGuid().ToString());
        var client = _factory.CreateClient();
        
        var response = await client.GetAsync(Products.ById.Uri(id.Value));
        
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Theory, TestPriority(9)]
    [LoadData("product")]
    public async Task add_should_create_new_product(Product product)
    {
        var client = _factory.CreateClient();
        var request = new CreateProductRequest(
            Guid.NewGuid(),
            Guid.NewGuid().ToString(),
            product.Brand,
            product.Model);
        
        var response = await client.PostAsJsonAsync(Products.Create.Uri(), request);

        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location?.ToString().ShouldStartWith(Products.Prefix);
    }
    
   
    [Theory, TestPriority(10)]
    [LoadData("product")]
    public async Task patch_should_update_product(Product product)
    {
        var client = _factory.CreateClient();
        var productId = new ProductId("C");
        var request = new UpdateProductRequest(Guid.NewGuid(), product.Brand);

        var response = await client.PatchAsJsonAsync(
            Products.Update.Uri(productId.Value), request);

        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Headers.Location?.ToString().ShouldContain(Products.Prefix);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProductResponse>(content);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(productId.Value);
        result.Brand.ShouldBe(product.Brand);
    }

    [Theory, TestPriority(11)]
    [LoadData("product")]
    public async Task get_events_by_id_should_return_product_events(Product product)
    {
        var client = _factory.CreateClient();
        var productId = new ProductId(Guid.NewGuid().ToString());
        var postRequest = new CreateProductRequest(
            Guid.NewGuid(),
            productId.Value,
            product.Brand,
            product.Model);
        var patchRequest = new UpdateProductRequest(Guid.NewGuid(), owner: new Owner(product.OwnerId!.Value));

        await client.PostAsJsonAsync(Products.Create.Uri(), postRequest);
        await client.PatchAsJsonAsync(Products.Update.Uri(productId.Value), patchRequest);
        var response = await client.GetAsync(Products.EventsById.Uri(productId.Value));
        
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProductEventsResponse>(content);
        result.ShouldNotBeNull();
        result.ProductCreatedEvent.ShouldNotBeNull();
        result.ProductOwnerChangedEvents.Last().OwnerId.ShouldBe(product.OwnerId!.Value);
    }

    [Theory, TestPriority(12)]
    [InlineData("C")]
    public async Task activate_should_update_existing_product(string id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.PatchAsync(Products.Activate.Uri(id) + $"?activateBy={Guid.NewGuid()}", null);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProductResponse>(content);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeTrue();
    }
    
    [Theory, TestPriority(13)]
    [InlineData("C")]
    public async Task deactivate_should_update_existing_product(string id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.PatchAsync(Products.Deactivate.Uri(id) + $"?deactivateBy={Guid.NewGuid()}", null);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProductResponse>(content);
        result.ShouldNotBeNull();
        result.IsActive.ShouldBeFalse();
    }

    [Theory, TestPriority(14)]
    [InlineData("B")]
    public async Task delete_should_remove_existing_product(string id)
    {
        var client = _factory.CreateClient();
        
        var response = await client.DeleteAsync(Products.Delete.Uri(id) + $"?deleteBy={Guid.NewGuid()}");
       
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}
