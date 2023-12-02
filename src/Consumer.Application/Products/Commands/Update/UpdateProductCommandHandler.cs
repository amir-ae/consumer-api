using Consumer.API.Contract.V1.Products.Messages;
using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Consumer.Application.Common.Interfaces.Services;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Application.Customers.Commands.Update;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.Events;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.Events;
using Consumer.Domain.Products.ValueObjects;

namespace Consumer.Application.Products.Commands.Update;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ErrorOr<Product>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderingService _orderingService;
    private readonly ISender _mediator;

    public UpdateProductCommandHandler(IProductRepository productRepository, ICustomerRepository customerRepository, IOrderingService orderingService, ISender mediator)
    {
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _orderingService = orderingService;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Product>> Handle(UpdateProductCommand command, CancellationToken ct = default)
    {
        var (productId, brand, model, owner, dealer, deviceType, 
            panelModel, panelSerialNumber, warrantyCardNumber, dateOfPurchase, 
            invoiceNumber, purchasePrice, orders, isUnrepairable, 
            dateOfDemandForCompensation, demanderFullName, updateBy, updateAt,
            onCreate, onChangingRole, version) = command;

        var product = await _productRepository.ByIdAsync(productId, ct);
        if (product is null) return Error.NotFound(
            nameof(ProductId), $"{nameof(UpsertProductCommand)} with id {productId} is not found.");

        Action<ProductEvent, int?> append = _productRepository.Append;
        Action<CustomerEvent, int?> customerAppend = _customerRepository.Append;
        int? customerVersion = null;
        ProductUpdateMessage? updateMessage = null;
        
        product = product.UpdateBrand(brand, updateBy, updateAt, append, ref version, out bool brandUpdated);
        if (brandUpdated)
        {
            updateMessage = new ProductUpdateMessage(productId.Value, 
                brand, null, updateBy.Value, updateAt);
        }
        
        product = product.UpdateModel(model, updateBy, updateAt, append, ref version, out bool modelUpdated);
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
        
        product = product.UpdateDeviceType(deviceType, updateBy, updateAt, append, ref version);

        product = product.UpdatePanel(panelModel, panelSerialNumber, updateBy, updateAt, append, ref version);

        product = product.UpdateWarrantyCardNumber(warrantyCardNumber, updateBy, updateAt, append, ref version);
        
        product = product.UpdatePurchaseData(dateOfPurchase, invoiceNumber, purchasePrice, updateBy, updateAt, append, ref version);

        product = product.UpdateUnrepairable(isUnrepairable, dateOfDemandForCompensation, demanderFullName, 
            updateBy, updateAt, append, ref version);

        product = product.AddOrders(orders, updateBy, updateAt, append, ref version);

        if (!onCreate)
        {
            product = product.RemoveOrders(orders, updateBy, updateAt, append, ref version);
        }

        var currentOwnerId = product.OwnerId;
        var currentDealerId = product.DealerId;
        var ownerId = owner?.CustomerId;
        var dealerId = dealer?.CustomerId;

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

        if (owner is not null)
        {
            Customer? newOwner;
            string? ownerName = null;
            if (ownerId is null)
            {
                var createCustomerCommand = new CreateCustomerCommand(
                    owner.FirstName ?? string.Empty,
                    owner.MiddleName ?? string.Empty,
                    owner.LastName ?? string.Empty,
                    owner.PhoneNumber ?? string.Empty,
                    owner.CityId ?? new CityId(default),
                    owner.Address ?? string.Empty,
                    owner.Role,
                    new HashSet<UpsertProductCommand> { new (productId) },
                    null,
                    updateBy,
                    updateAt);
                
                var createResult = await _mediator.Send(createCustomerCommand, ct);
                if (createResult.IsError) return createResult.Errors;
                newOwner = createResult.Value;
                ownerId = newOwner.Id;
            }
            else if (ownerId.Value != string.Empty)
            {
                if (owner.IsId)
                {
                    newOwner = await _customerRepository.ByIdAsync(ownerId, ct);
                    if (newOwner is null) return Error.NotFound(
                        nameof(CustomerId), $"{nameof(UpsertCustomerCommand)} with id {ownerId} is not found.");
                }
                else
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
                        true);
                
                    var updateResult = await _mediator.Send(updateCustomer, ct);
                    if (updateResult.IsError) return updateResult.Errors;
                    newOwner = updateResult.Value;
                }

                ownerName = newOwner.FullName;
                newOwner.AddProduct(productId, updateBy, updateAt, customerAppend, ref customerVersion);
            }
            
            product = product.UpdateOwner(ownerId, ownerName, updateBy, updateAt, append, ref version, out bool ownerUpdated);
            if (ownerUpdated && currentOwnerId is not null && !onChangingRole)
            {
                var currentOwner = await _customerRepository.ByIdAsync(currentOwnerId, ct);
                currentOwner?.RemoveProduct(productId, updateBy, updateAt, customerAppend, ref customerVersion);
            }
        }

        if (dealer is not null)
        {
            Customer? newDealer;
            string? dealerName = null;
            if (dealerId is null)
            {
                var createCustomer = new CreateCustomerCommand(
                    dealer.FirstName ?? string.Empty,
                    dealer.MiddleName ?? string.Empty,
                    dealer.LastName ?? string.Empty,
                    dealer.PhoneNumber ?? string.Empty,
                    dealer.CityId ?? new CityId(default),
                    dealer.Address ?? string.Empty,
                    dealer.Role,
                    new HashSet<UpsertProductCommand> { new (productId) },
                    null,
                    updateBy,
                    updateAt);
                
                var createResult = await _mediator.Send(createCustomer, ct);
                if (createResult.IsError) return createResult.Errors;
                newDealer = createResult.Value;
                dealerId = newDealer.Id;
            }
            else if (dealerId.Value != string.Empty)
            {
                if (dealer.IsId)
                {
                    newDealer = await _customerRepository.ByIdAsync(dealerId, ct);
                    if (newDealer is null) return Error.NotFound(
                        nameof(CustomerId), $"{nameof(UpsertCustomerCommand)} with id {dealerId} is not found.");
                }
                else
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
                        true);

                    var updateResult = await _mediator.Send(updateCustomer, ct);
                    if (updateResult.IsError) return updateResult.Errors;
                    newDealer = updateResult.Value;
                }
                
                dealerName = newDealer.FullName;
                newDealer.AddProduct(productId, updateBy, updateAt, customerAppend, ref customerVersion);
            }
            
            product = product.UpdateDealer(dealerId, dealerName, updateBy, updateAt, append, ref version, out bool dealerUpdated);
            if (dealerUpdated && currentDealerId is not null && !onChangingRole)
            {
                var currentDealer = await _customerRepository.ByIdAsync(currentDealerId, ct);
                currentDealer?.RemoveProduct(productId, updateBy, updateAt, customerAppend, ref customerVersion);
            }
        }

        if (updateMessage is not null)
        {
            await _orderingService.PublishProductUpdateAsync(updateMessage, ct);
        }
        
        await _productRepository.SaveChangesAsync(ct);

        return product;
    }
}