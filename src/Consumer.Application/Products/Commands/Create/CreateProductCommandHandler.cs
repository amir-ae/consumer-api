using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Products.Commands.Update;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.Events;
using Mapster;

namespace Consumer.Application.Products.Commands.Create;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ErrorOr<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ISender _mediator;

    public CreateProductCommandHandler(IProductRepository productRepository, ICustomerRepository customerRepository,  ISender mediator)
    {
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _mediator = mediator;
    }

    public async Task<ErrorOr<ProductResponse>> Handle(CreateProductCommand command, CancellationToken ct = default)
    {
        var (appUserId, productId, brand, model, serialId, ownerId, dealerId, orders, 
            deviceType, panelModel, panelSerialNumber, warrantyCardNumber, 
            dateOfPurchase, invoiceNumber, purchasePrice, unrepairable, 
            dateOfDemandForCompensation, demanderFullName, createdAt) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);

        if (product is not null)
        {
            if (product.IsDeleted == true)
            {
                var productUndeletedEvent = new ProductUndeletedEvent(
                    productId,
                    appUserId,
                    createdAt);

                await _productRepository.UndeleteAsync(productUndeletedEvent, ct);
            }
            
            var updateCommand = new UpdateProductCommand(
                appUserId,
                productId,
                brand,
                model,
                ownerId,
                dealerId,
                orders,
                deviceType,
                panelModel,
                panelSerialNumber,
                warrantyCardNumber,
                dateOfPurchase,
                invoiceNumber,
                purchasePrice,
                unrepairable,
                dateOfDemandForCompensation,
                demanderFullName,
                createdAt,
                true);
                
            return await _mediator.Send(updateCommand, ct);
        }

        string? ownerName = null; 
        if (ownerId is not null)
        {
            var owner = await _customerRepository.ByIdAsync(ownerId, ct);
        
            if (owner is null) return Error.NotFound(
                nameof(CustomerId), $"{nameof(Customer)} with id {ownerId} is not found");

            ownerName = owner.FullName;
            
            var customerProductAddedEvent = new CustomerProductAddedEvent(
                ownerId,
                productId,
                appUserId,
                createdAt);
        
            await _customerRepository.UpdateAsync(customerProductAddedEvent, ct: ct);
        }
        
        string? dealerName = null; 
        if (dealerId is not null)
        {
            var dealer = await _customerRepository.ByIdAsync(dealerId, ct);
        
            if (dealer is null) return Error.NotFound(
                nameof(CustomerId), $"{nameof(Customer)} with id {ownerId} is not found");

            dealerName = dealer.FullName;
            
            var customerProductAddedEvent = new CustomerProductAddedEvent(
                dealerId,
                productId,
                appUserId,
                createdAt);
        
            await _customerRepository.UpdateAsync(customerProductAddedEvent, ct: ct);
        }
        
        var productCreatedEvent = new ProductCreatedEvent(
            productId,
            brand,
            model,
            serialId,
            ownerId,
            ownerName,
            dealerId,
            dealerName,
            orders,
            deviceType,
            panelModel,
            panelSerialNumber,
            warrantyCardNumber,
            dateOfPurchase,
            invoiceNumber,
            purchasePrice,
            unrepairable,
            dateOfDemandForCompensation,
            demanderFullName,
            appUserId,
            createdAt);
        
        var productResult = await _productRepository.CreateAsync(productCreatedEvent, ct);

        return productResult.Adapt<ProductResponse>();
    }
}