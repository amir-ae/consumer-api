using Consumer.API.Contract.V1.Products.Messages;
using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Application.Customers.Commands.Update;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Consumer.Application.Products.Commands.Update;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ErrorOr<Product>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IEventSubscriptionManager _subscriptionManager;
    private readonly ISender _mediator;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IEventSubscriptionManager subscriptionManager, ISender mediator)
    {
        _unitOfWork = unitOfWork;
        _productRepository = unitOfWork.ProductRepository;
        _customerRepository = unitOfWork.CustomerRepository;
        _subscriptionManager = subscriptionManager;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Product>> Handle(UpdateProductCommand command, CancellationToken ct = default)
    {
        var (productId, brand, model, owner, dealer, deviceType, 
            panelModel, panelSerialNumber, warrantyCardNumber, dateOfPurchase, 
            invoiceNumber, purchasePrice, orders, isUnrepairable, 
            dateOfDemandForCompensation, demanderFullName, updateBy, updateAt,
            saveChanges, isOnCreate, isOnChangingRole, version) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        if (product is null) return Error.NotFound(
            nameof(ProductId), $"{nameof(Product)} with id {productId} is not found.");
        
        if (version.HasValue && version != product.Version) return Error.Conflict(
            nameof(StatusCodes.Status412PreconditionFailed), $"{nameof(product.Version)} mismatch.");
        
        _subscriptionManager.SubscribeToProductEvents(product);
        ProductUpdateMessage? updateMessage = null;
        
        product.UpdateBrand(brand, updateBy, updateAt, out bool brandUpdated);
        if (brandUpdated)
        {
            updateMessage = new ProductUpdateMessage(productId.Value, 
                brand, null, updateBy.Value, updateAt);
        }
        
        product.UpdateModel(model, updateBy, updateAt, out bool modelUpdated);
        if (modelUpdated)
        {
            if (updateMessage is null)
            {
                updateMessage = new ProductUpdateMessage(productId.Value, null, model, updateBy.Value, updateAt);
            }
            else
            {
                updateMessage = updateMessage with { Model = model };
            }
        }
        
        product.UpdateDeviceType(deviceType, updateBy, updateAt);

        product.UpdatePanel(panelModel, panelSerialNumber, updateBy, updateAt);

        product.UpdateWarrantyCardNumber(warrantyCardNumber, updateBy, updateAt);
        
        product.UpdatePurchaseData(dateOfPurchase, invoiceNumber, purchasePrice, updateBy, updateAt);

        product.UpdateUnrepairable(isUnrepairable, dateOfDemandForCompensation, demanderFullName, 
            updateBy, updateAt);

       product.AddOrders(orders, updateBy, updateAt);

        if (!isOnCreate)
        {
            product.RemoveOrders(orders, updateBy, updateAt);
        }
        
        List<Customer> customers = new();
        var currentOwnerId = product.OwnerId;
        var currentDealerId = product.DealerId;
        var ownerId = owner?.CustomerId;
        var dealerId = dealer?.CustomerId;

        if (isOnChangingRole)
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

        if (owner is not null)
        {
            Customer? newOwner = null;
            if (ownerId is null)
            {
                if (!owner.IsId)
                {
                    var createCustomerCommand = new CreateCustomerCommand(
                        owner.FirstName!,
                        owner.MiddleName!,
                        owner.LastName!,
                        owner.PhoneNumber!,
                        owner.CityId!,
                        owner.Address!,
                        owner.Role,
                        new HashSet<UpsertProductCommand> { new (productId) },
                        null,
                        updateBy,
                        updateAt,
                        false);
                
                    var createResult = await _mediator.Send(createCustomerCommand, ct);
                    if (createResult.IsError) return createResult.Errors;
                    newOwner = createResult.Value;
                    ownerId = newOwner.Id;
                }
            }
            else if (ownerId.Value != string.Empty)
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
                        null,
                        null,
                        updateBy,
                        updateAt,
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
                newOwner.AddProduct(productId, updateBy, updateAt);
            }
            var ownerName = newOwner?.FullName;

            product.UpdateOwner(ownerId, ownerName, updateBy, updateAt, out bool ownerUpdated);
            if (ownerUpdated && currentOwnerId is not null && !isOnChangingRole)
            {
                var currentOwner = await _customerRepository.ByIdAsync(currentOwnerId, ct);
                if (currentOwner is not null)
                {
                    customers.Add(currentOwner);
                    _subscriptionManager.SubscribeToCustomerEvents(currentOwner);
                    currentOwner.RemoveProduct(productId, updateBy, updateAt);
                }
            }
        }

        if (dealer is not null)
        {
            Customer? newDealer = null;
            if (dealerId is null)
            {
                if (!dealer.IsId)
                {
                    var createCustomer = new CreateCustomerCommand(
                        dealer.FirstName!,
                        dealer.MiddleName!,
                        dealer.LastName!,
                        dealer.PhoneNumber!,
                        dealer.CityId!,
                        dealer.Address!,
                        dealer.Role,
                        new HashSet<UpsertProductCommand> { new (productId) },
                        null,
                        updateBy,
                        updateAt,
                        false);
                
                    var createResult = await _mediator.Send(createCustomer, ct);
                    if (createResult.IsError) return createResult.Errors;
                    newDealer = createResult.Value;
                    dealerId = newDealer.Id;
                }
            }
            else if (dealerId.Value != string.Empty)
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
                        null,
                        null,
                        updateBy,
                        updateAt,
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
                newDealer.AddProduct(productId, updateBy, updateAt);
            }
            var dealerName = newDealer?.FullName;

            product.UpdateDealer(dealerId, dealerName, updateBy, updateAt, out bool dealerUpdated);
            if (dealerUpdated && currentDealerId is not null && !isOnChangingRole)
            {
                var currentDealer = await _customerRepository.ByIdAsync(currentDealerId, ct);
                if (currentDealer is not null)
                {
                    customers.Add(currentDealer);
                    _subscriptionManager.SubscribeToCustomerEvents(currentDealer);
                    currentDealer.RemoveProduct(productId, updateBy, updateAt);
                }
            }
        }

        if (product.Orders.Any() && updateMessage is not null)
        {
            _unitOfWork.ProductUpdateMessages.Add(updateMessage);
        }

        if (!saveChanges)
        {
            return product;
        }
        
        await _unitOfWork.SaveChangesAsync(ct);
        _subscriptionManager.UnsubscribeFromProductEvents(product);
        customers.ForEach(customer => _subscriptionManager.UnsubscribeFromCustomerEvents(customer));

        var result = await _productRepository.ByStreamIdAsync(productId, ct);
        if (result is null) return Error.Unexpected(
            nameof(ProductId), $"{nameof(Product)} stream with id {productId} is not found.");

        return result;
    }
}