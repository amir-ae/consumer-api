using Consumer.Application.Common.Commands;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
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
    private readonly ISender _mediator;

    public CreateProductCommandHandler(IProductRepository productRepository, ICustomerRepository customerRepository,  ISender mediator)
    {
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _mediator = mediator;
    }

    public async Task<ErrorOr<Product>> Handle(CreateProductCommand command, CancellationToken ct = default)
    {
        var (productId, brand, model, serialId, owner, dealer, 
            deviceType, panelModel, panelSerialNumber, warrantyCardNumber, 
            dateOfPurchase, invoiceNumber, purchasePrice, orders, unrepairable, 
            dateOfDemandForCompensation, demanderFullName, createBy, createAt) = command;

        Func<ProductCreatedEvent, CancellationToken, Task<Product>> create = _productRepository.CreateAsync;
        Action<ProductEvent> append = _productRepository.Append;
        Action<CustomerEvent> customerAppend = _customerRepository.Append;

        var ownerId = owner?.CustomerId;
        var dealerId = dealer?.CustomerId;

        string? ownerName = null;
        if (owner is not null && !owner.IsId)
        {
            Customer? newOwner;
            if (ownerId is null)
            {
                var createCustomer = new CreateCustomerCommand(
                    owner.FirstName ?? string.Empty,
                    owner.MiddleName ?? string.Empty,
                    owner.LastName ?? string.Empty,
                    owner.PhoneNumber ?? string.Empty,
                    owner.CityId ?? new CityId(default),
                    owner.Address ?? string.Empty,
                    owner.Role,
                    new HashSet<UpsertProductCommand> { new (productId) },
                    null,
                    createBy,
                    createAt);
                
                var createResult = await _mediator.Send(createCustomer, ct);
                if (createResult.IsError) return createResult.Errors;
                newOwner = createResult.Value;
                ownerId = newOwner.Id;
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
                    new HashSet<UpsertProductCommand> { new (productId) },
                    null,
                    createBy,
                    createAt,
                    true);
                
                var updateResult = await _mediator.Send(updateCustomer, ct);
                if (updateResult.IsError) return updateResult.Errors;
                newOwner = updateResult.Value;
            }
            
            ownerName = newOwner.FullName;
            newOwner.AddProduct(productId, createBy, createAt, customerAppend);
        }
        
        string? dealerName = null;
        if (dealer is not null && !dealer.IsId)
        {
            Customer? newDealer;
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
                    createBy,
                    createAt);
                
                var createResult = await _mediator.Send(createCustomer, ct);
                if (createResult.IsError) return createResult.Errors;
                newDealer = createResult.Value;
                dealerId = newDealer.Id;
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
                    new HashSet<UpsertProductCommand> { new (productId) },
                    null,
                    createBy,
                    createAt,
                    true);
                
                var updateResult = await _mediator.Send(updateCustomer, ct);
                if (updateResult.IsError) return updateResult.Errors;
                newDealer = updateResult.Value;
            }

            dealerName = newDealer.FullName;
            newDealer.AddProduct(productId, createBy, createAt, customerAppend);
        }
        
        var product = await _productRepository.ByIdAsync(productId, ct);

        if (product is null)
        {
            return await Product.CreateAsync(productId,
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
                create, 
                ct);
        }
        
        product.Undelete(createBy, append);
            
        var updateProduct = new UpdateProductCommand(
            productId,
            brand,
            model,
            ownerId is null ? null : new UpsertCustomerCommand(ownerId),
            dealerId is null ? null : new UpsertCustomerCommand(dealerId),
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
            true);
                
        return await _mediator.Send(updateProduct, ct);
    }
}