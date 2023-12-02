using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Application.Customers.Commands.Update;
using Consumer.Application.Products.Commands.Update;
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
        var (appUserId, productId, brand, model, serialId, owner, dealer, 
            deviceType, panelModel, panelSerialNumber, warrantyCardNumber, 
            dateOfPurchase, invoiceNumber, purchasePrice, orders, unrepairable, 
            dateOfDemandForCompensation, demanderFullName, createdAt) = command;

        var ownerId = owner?.CustomerId;
        var dealerId = dealer?.CustomerId;

        string? ownerName = null;
        if (owner is not null)
        {
            if (ownerId is null)
            {
                var createCustomerCommand = new CreateCustomerCommand(
                    appUserId,
                    owner.FirstName ?? string.Empty,
                    owner.MiddleName ?? string.Empty,
                    owner.LastName ?? string.Empty,
                    owner.PhoneNumber ?? string.Empty,
                    owner.CityId ?? new CityId(default),
                    owner.Address ?? string.Empty,
                    owner.Role,
                    new HashSet<Product> { new (productId) },
                    createdAt);
                
                var ownerResult = await _mediator.Send(createCustomerCommand, ct);
                ownerName = ownerResult.Value.FullName;
                ownerId = new CustomerId(ownerResult.Value.CustomerId);
            }
            else
            {
                var updateCustomerCommand = new UpdateCustomerCommand(
                    appUserId,
                    ownerId,
                    owner.FirstName,
                    owner.MiddleName,
                    owner.LastName,
                    owner.PhoneNumber,
                    owner.CityId,
                    owner.Address,
                    owner.Role,
                    new HashSet<Product> { new (productId) },
                    createdAt,
                    true);
                
                var ownerResult = await _mediator.Send(updateCustomerCommand, ct);
                ownerName = ownerResult.Value.FullName;
            }

            var customerProductAddedEvent = new CustomerProductAddedEvent(
                ownerId,
                productId,
                appUserId,
                createdAt);
        
            await _customerRepository.UpdateAsync(customerProductAddedEvent, ct: ct);
        }
        
        string? dealerName = null;
        if (dealer is not null)
        {
            if (dealerId is null)
            {
                var createCustomerCommand = new CreateCustomerCommand(
                    appUserId,
                    dealer.FirstName ?? string.Empty,
                    dealer.MiddleName ?? string.Empty,
                    dealer.LastName ?? string.Empty,
                    dealer.PhoneNumber ?? string.Empty,
                    dealer.CityId ?? new CityId(default),
                    dealer.Address ?? string.Empty,
                    dealer.Role,
                    new HashSet<Product> { new (productId) },
                    createdAt);
                
                var dealerResult = await _mediator.Send(createCustomerCommand, ct);
                dealerName = dealerResult.Value.FullName;
                dealerId = new CustomerId(dealerResult.Value.CustomerId);
            }
            else
            {
                var updateCustomerCommand = new UpdateCustomerCommand(
                    appUserId,
                    dealerId,
                    dealer.FirstName,
                    dealer.MiddleName,
                    dealer.LastName,
                    dealer.PhoneNumber,
                    dealer.CityId,
                    dealer.Address,
                    dealer.Role,
                    new HashSet<Product> { new (productId) },
                    createdAt,
                    true);
                
                var dealerResult = await _mediator.Send(updateCustomerCommand, ct);
                dealerName = dealerResult.Value.FullName;
            }

            var customerProductAddedEvent = new CustomerProductAddedEvent(
                dealerId,
                productId,
                appUserId,
                createdAt);
        
            await _customerRepository.UpdateAsync(customerProductAddedEvent, ct: ct);
        }
        
        var product = await _productRepository.ByIdAsync(productId, ct);

        if (product is not null)
        {
            if (product.IsDeleted)
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
                new Customer(ownerId),
                new Customer(dealerId),
                deviceType,
                panelModel,
                panelSerialNumber,
                warrantyCardNumber,
                dateOfPurchase,
                invoiceNumber,
                purchasePrice,
                orders,
                unrepairable,
                dateOfDemandForCompensation,
                demanderFullName,
                createdAt,
                true);
                
            return await _mediator.Send(updateCommand, ct);
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
            deviceType,
            panelModel,
            panelSerialNumber,
            warrantyCardNumber,
            dateOfPurchase,
            invoiceNumber,
            purchasePrice,
            orders,
            unrepairable,
            dateOfDemandForCompensation,
            demanderFullName,
            appUserId,
            createdAt);
        
        var productResult = await _productRepository.CreateAsync(productCreatedEvent, ct);

        return productResult.Adapt<ProductResponse>();
    }
}