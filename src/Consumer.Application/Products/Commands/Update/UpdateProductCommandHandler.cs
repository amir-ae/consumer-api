using Consumer.API.Contract.V1.Products.Messages;
using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;
using Mapster;

namespace Consumer.Application.Products.Commands.Update;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ErrorOr<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderingService _orderingService;

    public UpdateProductCommandHandler(IProductRepository productRepository, ICustomerRepository customerRepository, IOrderingService orderingService)
    {
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _orderingService = orderingService;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(UpdateProductCommand command, CancellationToken ct = default)
    {
        var (appUserId, productId, brand, model, ownerId, dealerId, orders, deviceType, 
            panelModel, panelSerialNumber, warrantyCardNumber, dateOfPurchase, 
            invoiceNumber, purchasePrice, isUnrepairable, dateOfDemandForCompensation,
            demanderFullName, updatedAt, onCreate, onChangingRole) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        
        if (product is null) return Error.NotFound(
            nameof(ProductId), $"{nameof(Product)} with id {productId} is not found");

        ProductUpdateMessage? productUpdateMessage = null;
        
        if (!string.IsNullOrEmpty(brand) && brand != product.Brand)
        {
            var productBrandChangedEvent = new ProductBrandChangedEvent(
                productId,
                brand,
                appUserId,
                updatedAt);
            
            product = await _productRepository.UpdateAsync(productBrandChangedEvent, product, ct);
            
            productUpdateMessage = new ProductUpdateMessage(appUserId.Value, productId.Value, brand, null, updatedAt);
        }

        if (!string.IsNullOrEmpty(model) && model != product.Model)
        {
            var productModelChangedEvent = new ProductModelChangedEvent(
                productId,
                model,
                appUserId,
                updatedAt);
            
            product = await _productRepository.UpdateAsync(productModelChangedEvent, product, ct);

            if (productUpdateMessage is null)
            {
                productUpdateMessage = new ProductUpdateMessage(appUserId.Value, productId.Value, null, model, updatedAt);
            }
            else
            {
                productUpdateMessage = productUpdateMessage with { Model = model };
            }
        }

        var currentOwnerId = product.OwnerId;
        var currentDealerId = product.DealerId;
        
        if (onChangingRole)
        {
            if (currentOwnerId != dealerId && ownerId?.Value == string.Empty)
            {
                ownerId = null;
            }

            if (currentDealerId != ownerId && dealerId?.Value == string.Empty)
            {
                dealerId = null;
            }
        }
        
        if (ownerId is not null)
        {
            Customer? owner = null;

            if (ownerId.Value != string.Empty)
            {
                owner = await _customerRepository.ByIdAsync(ownerId, ct);
                if (owner is null) return Error.NotFound(
                    nameof(CustomerId), $"{nameof(Customer)} with id {ownerId} is not found");

                if (!owner.ProductIds.Contains(productId))
                {
                    var customerProductAddedEvent = new CustomerProductAddedEvent(
                        ownerId,
                        productId,
                        appUserId,
                        updatedAt);

                    await _customerRepository.UpdateAsync(customerProductAddedEvent, owner, ct);
                }
            }

            if (ownerId != currentOwnerId)
            {
                if (currentOwnerId is not null)
                {
                    var currentOwner = await _customerRepository.ByIdAsync(currentOwnerId, ct);

                    var customerProductRemovedEvent = new CustomerProductRemovedEvent(
                        currentOwnerId,
                        productId,
                        appUserId,
                        updatedAt);

                    await _customerRepository.UpdateAsync(customerProductRemovedEvent, currentOwner, ct);
                }
                
                var productOwnerChangedEvent = new ProductOwnerChangedEvent(
                    productId,
                    ownerId.Value == string.Empty ? null : ownerId,
                    owner?.FullName,
                    appUserId,
                    updatedAt);
            
                product = await _productRepository.UpdateAsync(productOwnerChangedEvent, product, ct);
            }
        }

        if (dealerId is not null)
        {
            Customer? dealer = null;

            if (dealerId.Value != string.Empty)
            {
                dealer = await _customerRepository.ByIdAsync(dealerId, ct);
                if (dealer is null) return Error.NotFound(
                    nameof(CustomerId), $"{nameof(Customer)} with id {dealerId} is not found");

                if (!dealer.ProductIds.Contains(productId))
                {
                    var customerProductAddedEvent = new CustomerProductAddedEvent(
                        dealerId,
                        productId,
                        appUserId,
                        updatedAt);

                    await _customerRepository.UpdateAsync(customerProductAddedEvent, dealer, ct);
                }
            }
            if (dealerId != currentDealerId)
            {
                if (currentDealerId is not null)
                {
                    var currentDealer = await _customerRepository.ByIdAsync(currentDealerId, ct);

                    var customerProductRemovedEvent = new CustomerProductRemovedEvent(
                        currentDealerId,
                        productId,
                        appUserId,
                        updatedAt);

                    await _customerRepository.UpdateAsync(customerProductRemovedEvent, currentDealer, ct);
                }
                
                var productDealerChangedEvent = new ProductDealerChangedEvent(
                    productId,
                    dealerId.Value == string.Empty ? null : dealerId,
                    dealer?.FullName,
                    appUserId,
                    updatedAt);
            
                product = await _productRepository.UpdateAsync(productDealerChangedEvent, product, ct);
            }
        }

        if (!string.IsNullOrEmpty(deviceType) && deviceType != product.DeviceType)
        {
            var productDeviceTypeChangedEvent = new ProductDeviceTypeChangedEvent(
                productId,
                deviceType,
                appUserId,
                updatedAt);
            
            product = await _productRepository.UpdateAsync(productDeviceTypeChangedEvent, product, ct);
        }

        if (!string.IsNullOrEmpty(panelModel) && panelModel != product.PanelModel
            || !string.IsNullOrEmpty(panelSerialNumber) && panelSerialNumber != product.PanelSerialNumber)
        {
            var productPanelChangedEvent = new ProductPanelChangedEvent(
                productId,
                panelModel ?? product.PanelModel,
                panelSerialNumber ?? product.PanelSerialNumber,
                appUserId,
                updatedAt);
            
            product = await _productRepository.UpdateAsync(productPanelChangedEvent, product, ct);
        }

        if (!string.IsNullOrEmpty(warrantyCardNumber) && warrantyCardNumber != product.WarrantyCardNumber)
        {
            var productWarrantyCardNumberChangedEvent = new ProductWarrantyCardNumberChangedEvent(
                productId,
                warrantyCardNumber,
                appUserId,
                updatedAt);
            
            product = await _productRepository.UpdateAsync(productWarrantyCardNumberChangedEvent, product, ct);
        }

        if (dateOfPurchase is not null && dateOfPurchase != product.DateOfPurchase
            || !string.IsNullOrEmpty(invoiceNumber) && invoiceNumber != product.InvoiceNumber
            || purchasePrice is not null && purchasePrice != product.PurchasePrice)
        {
            var productPurchaseDataChangedEvent = new ProductPurchaseDataChangedEvent(
                productId,
                dateOfPurchase ?? product.DateOfPurchase,
                invoiceNumber ?? product.InvoiceNumber,
                purchasePrice ?? product.PurchasePrice,
                appUserId,
                updatedAt);
            
            product = await _productRepository.UpdateAsync(productPurchaseDataChangedEvent, product, ct);
        }
        
        if (isUnrepairable is not null && isUnrepairable != product.IsUnrepairable
            || dateOfDemandForCompensation is not null && dateOfDemandForCompensation != product.DateOfDemandForCompensation
            || !string.IsNullOrEmpty(demanderFullName) && demanderFullName != product.DemanderFullName)
        {
            var productUnrepairableEvent = new ProductUnrepairableEvent(
                productId,
                isUnrepairable ?? product.IsUnrepairable,
                dateOfDemandForCompensation ?? product.DateOfDemandForCompensation,
                demanderFullName ?? product.DemanderFullName,
                appUserId,
                updatedAt);
            
            product = await _productRepository.UpdateAsync(productUnrepairableEvent, product, ct);
        }
        
        if (orders is not null && !orders.SetEquals(product.Orders))
        {
            var ordersToAdd = orders.Except(product.Orders);

            foreach (var orderId in ordersToAdd)
            {
                var productOrderAddedEvent = new ProductOrderAddedEvent(
                    productId,
                    orderId,
                    appUserId,
                    updatedAt);
            
                product = await _productRepository.UpdateAsync(productOrderAddedEvent, product, ct);
            }

            if (!onCreate)
            {
                var ordersToRemove = product.Orders.Except(orders);
                foreach (var orderId in ordersToRemove)
                {
                    var productOrderRemovedEvent = new ProductOrderRemovedEvent(
                        productId,
                        orderId,
                        appUserId,
                        updatedAt);
            
                    product = await _productRepository.UpdateAsync(productOrderRemovedEvent, product, ct);
                }
            }
        }

        if (productUpdateMessage is not null)
        {
            await _orderingService.PublishProductUpdateAsync(productUpdateMessage, ct);
        }

        await _customerRepository.SaveChangesAsync(ct);
        await _productRepository.SaveChangesAsync(ct);

        return product.Adapt<ProductResponse>();
    }
}