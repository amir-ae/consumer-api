using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Application.Customers.Commands.Update;
using Consumer.Application.Products.Commands.Update;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;

namespace Consumer.Application.Products.Commands.Create;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ISender _mediator;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork, IEventSubscriptionManager subscriptionManager, ISender mediator)
    {
        _productRepository = unitOfWork.ProductRepository;
        _customerRepository = unitOfWork.CustomerRepository;
        _subscriptionManager = subscriptionManager;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Product>> Handle(CreateProductCommand command, CancellationToken ct = default)
    {
        var (productId, brand, model, serialId, owner, dealer, 
            deviceType, panelModel, panelSerialNumber, warrantyCardNumber, 
            dateOfPurchase, invoiceNumber, purchasePrice, orders, unrepairable, 
            dateOfDemandForCompensation, demanderFullName, createBy, createAt, saveChanges) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        
        if (product is not null)
        {
            _subscriptionManager.SubscribeToProductEvents(product);
            product.Undelete(createBy);
            
            var updateProduct = new UpdateProductCommand(
                productId,
                brand,
                model,
                owner,
                dealer,
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
                createBy,
                createAt,
                saveChanges,
                true);
                
            return await _mediator.Send(updateProduct, ct);
        }

        List<Customer> customers = new();
        var ownerId = owner?.CustomerId;
        var dealerId = dealer?.CustomerId;

        string? ownerName = null;
        if (owner is not null)
        {
            Customer? newOwner = null;
            if (ownerId is null)
            {
                if (!owner.IsId)
                {
                    var createCustomer = new CreateCustomerCommand(
                        owner.FirstName!,
                        owner.MiddleName,
                        owner.LastName!,
                        owner.PhoneNumber!,
                        owner.CityId!,
                        owner.Address!,
                        owner.Role,
                        new HashSet<UpsertProductCommand> { new (productId) },
                        null,
                        createBy,
                        createAt,
                        false);
                
                    var createResult = await _mediator.Send(createCustomer, ct);
                    if (createResult.IsError) return createResult.Errors;
                    newOwner = createResult.Value;
                    ownerId = newOwner.Id;
                }
            }
            else
            {
                if (!owner.IsId)
                {
                    var updateCustomer = new UpdateCustomerCommand(
                        ownerId,
                        owner.FirstName,
                        owner.MiddleName,
                        owner.LastName,
                        owner.PhoneNumber,
                        owner.CityId,
                        owner.Address,
                        owner.Role,
                        new HashSet<UpsertProductCommand> { new (productId) },
                        null,
                        createBy,
                        createAt,
                        false,
                        true);
                
                    var updateResult = await _mediator.Send(updateCustomer, ct);
                    if (updateResult.IsError) return updateResult.Errors;
                    newOwner = updateResult.Value;
                }
                else
                {
                    newOwner = await _customerRepository.ByIdAsync(ownerId, ct);
                    if (newOwner is null) return Error.NotFound(
                        nameof(CustomerId), $"{nameof(UpsertCustomerCommand)} with id {ownerId} is not found.");
                }
            }

            if (newOwner is not null)
            {
                customers.Add(newOwner);     
                _subscriptionManager.SubscribeToCustomerEvents(newOwner);
                newOwner.AddProduct(productId, createBy, createAt);
            }
            ownerName = newOwner?.FullName;
        }
        
        string? dealerName = null;
        if (dealer is not null)
        {
            Customer? newDealer = null;
            if (dealerId is null)
            {
                if (!dealer.IsId)
                {
                    var createCustomer = new CreateCustomerCommand(
                        dealer.FirstName!,
                        dealer.MiddleName,
                        dealer.LastName!,
                        dealer.PhoneNumber!,
                        dealer.CityId!,
                        dealer.Address!,
                        dealer.Role,
                        new HashSet<UpsertProductCommand> { new (productId) },
                        null,
                        createBy,
                        createAt,
                        false);
                
                    var createResult = await _mediator.Send(createCustomer, ct);
                    if (createResult.IsError) return createResult.Errors;
                    newDealer = createResult.Value;
                    dealerId = newDealer.Id;
                }
            }
            else
            {
                if (!dealer.IsId)
                {
                    var updateCustomer = new UpdateCustomerCommand(
                        dealerId,
                        dealer.FirstName,
                        dealer.MiddleName,
                        dealer.LastName,
                        dealer.PhoneNumber,
                        dealer.CityId,
                        dealer.Address,
                        dealer.Role,
                        new HashSet<UpsertProductCommand> { new (productId) },
                        null,
                        createBy,
                        createAt,
                        false,
                        true);
                
                    var updateResult = await _mediator.Send(updateCustomer, ct);
                    if (updateResult.IsError) return updateResult.Errors;
                    newDealer = updateResult.Value;
                }
                else
                {
                    newDealer = await _customerRepository.ByIdAsync(dealerId, ct);
                    if (newDealer is null) return Error.NotFound(
                        nameof(CustomerId), $"{nameof(UpsertCustomerCommand)} with id {dealerId} is not found.");
                }
            }

            if (newDealer is not null)
            {
                customers.Add(newDealer);
                _subscriptionManager.SubscribeToCustomerEvents(newDealer);
                newDealer.AddProduct(productId, createBy, createAt);
            }
            dealerName = newDealer?.FullName;
        }

        var result = await Product.CreateAsync(productId,
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
            createBy,
            createAt,
            _productRepository.Create,
            _productRepository.CreateAsync, 
            saveChanges,
            ct);
        
        customers.ForEach(customer => _subscriptionManager.UnsubscribeFromCustomerEvents(customer));

        return result;
    }
}