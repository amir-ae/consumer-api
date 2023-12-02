using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;
using Consumer.Fixtures;
using Consumer.Infrastructure.Persistence.Repositories;
using Shouldly;
using Xunit;

namespace Consumer.Infrastructure.Tests;

public class ProductRepositoryTests : IntegrationTest
{
    private readonly ProductRepository _sut;

    public ProductRepositoryTests(ConsumerApplicationFactory<Program> factory) : base(factory)
    {
        _sut = new ProductRepository(OpenSession());
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
        result.All(p => p.Owner != null).ShouldBeTrue();
    }
    
    [Theory]
    [InlineData("3")]
    public async Task should_not_return_deleted_records(string id)
    {
        var result = await _sut.AllAsync();

        result.Any(x => x.Id == new ProductId(id)).ShouldBeFalse();
    }
    
    [Theory]
    [LoadData("product")]
    public async Task should_return_record_detail_by_order_id(Product product)
    {
        var orderId = product.Orders.First().OrderId;
        
        var result = await _sut.DetailByOrderIdAsync(orderId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(product.Id);
        result.Owner.ShouldNotBeNull();
    }
    
    [Theory]
    [LoadData("product")]
    public async Task should_return_records_detail_by_centre_id(Product product)
    {
        var centreId = product.Orders.First().CentreId;
        
        var result = await _sut.DetailByCentreIdAsync(centreId);

        result.ShouldNotBeEmpty();
        result.Select(p => p.Id).ShouldContain(product.Id);
        result.ForEach(p => p.Orders.Select(o => o.CentreId).ShouldContain(centreId));
        result.All(p => p.Owner != null).ShouldBeTrue();
    }
    
    [Theory]
    [LoadData("product.Id")]
    public async Task should_return_record_by_id(ProductId id)
    {
        var result = await _sut.ByIdAsync(id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
    }

    [Theory]
    [LoadData("product.Id")]
    public async Task should_return_record_detail_by_id(ProductId id)
    {
        var result = await _sut.DetailByIdAsync(id);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(id);
        result.Owner.ShouldNotBeNull();
    }

    [Fact]
    public async Task should_return_null_with_id_not_present()
    {
        var productId = new ProductId(Guid.NewGuid().ToString());
        var result = await _sut.ByIdAsync(productId);

        result.ShouldBeNull();
    }

    [Theory]
    [LoadData("product.Id")]
    public async Task should_return_true_on_check_by_id(ProductId id)
    {
        var result = await _sut.CheckByIdAsync(id);
        result.ShouldBeTrue();
    }
    
    [Fact]
    public async Task should_return_false_on_check_by_id_not_present()
    {
        var productId = new ProductId(Guid.NewGuid().ToString());
        var result = await _sut.CheckByIdAsync(productId);
        result.ShouldBeFalse();
    }

    [Theory]
    [LoadData("product")]
    public async Task should_add_new_product(Product product)
    {
        var productId = new ProductId(Guid.NewGuid().ToString());
        var productCreatedEvent = new ProductCreatedEvent(
            productId,
            product.Model,
            product.Brand,
            null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null,
            new AppUserId(Guid.NewGuid())
        );
        
        await _sut.CreateAsync(productCreatedEvent);
        var result = await _sut.ByIdAsync(productId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(productId);
    }
    
    [Theory]
    [LoadData("product")]
    public async Task should_update_a_product(Product product)
    {
        var productId = new ProductId("C");
        var productBrandChangedEvent = new ProductBrandChangedEvent(
            productId,
            product.Brand,
            new AppUserId(Guid.NewGuid())
        );
        
        await _sut.UpdateAsync(productBrandChangedEvent);
        var result = await _sut.ByIdAsync(productId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(productId);
        result.Brand.ShouldBe(product.Brand);
    }
    
    [Theory]
    [LoadData("product")]
    public async Task should_return_record_events_by_id(Product product)
    {
        var productId = new ProductId(Guid.NewGuid().ToString());
        var productCreatedEvent = new ProductCreatedEvent(
            productId,
            product.Model,
            product.Brand,
            null, null, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null,
            new AppUserId(Guid.NewGuid())
        );
        var productOwnerChangedEvent = new ProductOwnerChangedEvent(
            productId,
            product.OwnerId,
            null,
            new AppUserId(Guid.NewGuid())
        );
        
        await _sut.CreateAsync(productCreatedEvent);
        await _sut.UpdateAsync(productOwnerChangedEvent);
        var result = await _sut.EventsByIdAsync(productId);

        result.ShouldNotBeNull();
        result.ProductCreatedEvent.ShouldNotBeNull();
        result.ProductOwnerChangedEvents.Last().OwnerId.ShouldBe(product.OwnerId);
    }
    
    [Theory]
    [InlineData("C")]
    public async Task should_activate_product(string id)
    {
        var productActivatedEvent = new ProductActivatedEvent(
            new ProductId(id),
            new AppUserId(Guid.NewGuid())
        );
        
        var result = await _sut.ActivateAsync(productActivatedEvent);
        
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(true);
    }
    
    [Theory]
    [InlineData("C")]
    public async Task should_deactivate_product(string id)
    {
        var productDeactivatedEvent = new ProductDeactivatedEvent(
            new ProductId(id),
            new AppUserId(Guid.NewGuid())
        );
        
        var result = await _sut.DeactivateAsync(productDeactivatedEvent);
        
        result.ShouldNotBeNull();
        result.IsActive.ShouldBe(false);
    }
    
    [Theory]
    [InlineData("B")]
    public async Task should_delete_product(string id)
    {
        var productDeletedEvent = new ProductDeletedEvent(
            new ProductId(id),
            new AppUserId(Guid.NewGuid())
        );
        
        var result = await _sut.DeleteAsync(productDeletedEvent);
        
        result.ShouldNotBeNull();
        result.IsDeleted.ShouldBe(true);
    }
}