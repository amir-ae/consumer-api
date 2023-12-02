using Consumer.API.Contract.V1;
using Consumer.API.Extensions.ErrorHandling;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Requests;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.Application.Products.Commands.Create;
using Consumer.Application.Products.Commands.Delete;
using Consumer.Application.Products.Commands.Update;
using Consumer.Application.Products.Commands.Activate;
using Consumer.Application.Products.Commands.Deactivate;
using Consumer.Application.Products.Queries.All;
using Consumer.Application.Products.Queries.AllDetail;
using Consumer.Application.Products.Queries.ByPage;
using Consumer.Application.Products.Queries.ById;
using Consumer.Application.Products.Queries.DetailByCentreId;
using Consumer.Application.Products.Queries.DetailById;
using Consumer.Application.Products.Queries.DetailByOrderId;
using Consumer.Application.Products.Queries.EventsById;
using Consumer.Application.Products.Queries.ByPageDetail;
using Consumer.Application.Products.Queries.CheckById;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products.ValueObjects;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Products = Consumer.API.Contract.V1.Routes.Products;
using Customers = Consumer.API.Contract.V1.Routes.Customers;

namespace Consumer.API.Endpoints.V1;

public static class ProductEndpoints
{
    public static void AddProductEndpoints(this WebApplication app)
    {
        var productsGroup = app.MapGroup(Products.Prefix)
            //.RequireAuthorization()
            //.AddEndpointFilter<ApiKeyEndpointFilter>()
            .WithTags(nameof(Products))
            .WithOpenApi();

        productsGroup.MapGet(Products.ByPage.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageIndex, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductsByPageQuery(pageSize ?? 10, pageIndex ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new ProductId(keyId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<PaginatedList<ProductResponse>>()
            .WithName(Products.ByPage.Name)
            .WithDescription(Products.ByPage.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.ByPageDetail.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageIndex, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductsByPageDetailQuery(pageSize ?? 10, pageIndex ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new ProductId(keyId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<PaginatedList<ProductResponse>>()
            .WithName(Products.ByPageDetail.Name)
            .WithDescription(Products.ByPageDetail.Description);

        productsGroup.MapGet(Products.All.Pattern,
                async (ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new AllProductsQuery();
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<List<ProductForListingResponse>>()
            .WithName(Products.All.Name)
            .WithDescription(Products.All.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.AllDetail.Pattern,
                async (ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new AllProductsDetailQuery();
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<List<ProductResponse>>()
            .WithName(Products.AllDetail.Name)
            .WithDescription(Products.AllDetail.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.ById.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<ProductResponse>()
            .WithName(Products.ById.Name)
            .WithDescription(Products.ById.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.DetailById.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductDetailByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<ProductResponse>()
            .WithName(Products.DetailById.Name)
            .WithDescription(Products.DetailById.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.EventsById.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductEventsByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<ProductEventsResponse>()
            .WithName(Products.EventsById.Name)
            .WithDescription(Products.EventsById.Description);
        
        productsGroup.MapGet(Products.Check.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new CheckProductByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<bool>()
            .WithName(Products.Check.Name)
            .WithDescription(Products.Check.Description);

        productsGroup.MapGet(Products.DetailByCentreId.Pattern,
                async ([FromRoute] Guid centreId, ISender mediator,
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductsDetailByCentreIdQuery(new CentreId(centreId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();;
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<List<ProductResponse>>()
            .WithName(Products.DetailByCentreId.Name)
            .WithDescription(Products.DetailByCentreId.Description);

        productsGroup.MapGet(Products.DetailByOrderId.Pattern,
                async ([FromRoute] string orderId, ISender mediator, IErrorHandler errorHandler,
                    CancellationToken ct) =>
                {
                    var query = new ProductDetailByOrderIdQuery(new OrderId(orderId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<ProductResponse>()
            .WithName(Products.DetailByOrderId.Name)
            .WithDescription(Products.DetailByOrderId.Description);

        productsGroup.MapPost(Products.Post.Pattern,
                async ([FromBody] PostProductRequest request, ISender mediator, IOutputCacheStore cache,
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<CreateProductCommand>();
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                        {
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            return Results.Created(new Uri(Products.ById.Uri(product.ProductId), UriKind.Relative),
                                product);
                        }, 
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces<ProductResponse>(StatusCodes.Status201Created)
            .WithName(Products.Post.Name)
            .WithDescription(Products.Post.Description);

        productsGroup.MapPatch(Products.Patch.Pattern,
                async ([FromRoute] string productId, [FromBody] PatchProductRequest request, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<UpdateProductCommand>() with { ProductId = new ProductId(productId) };
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                        {
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            return Results.Ok(product);
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces<ProductResponse>()
            .WithName(Products.Patch.Name)
            .WithDescription(Products.Patch.Description);

        productsGroup.MapPatch(Products.Activate.Pattern,
                async ([FromRoute] string productId, [FromQuery] Guid appUserId, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new ActivateProductCommand(new AppUserId(appUserId), new ProductId(productId));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                    {
                        await cache.EvictByTagAsync(nameof(Products), ct);
                        return Results.Ok(product);
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces<ProductResponse>()
            .WithName(Products.Activate.Name)
            .WithDescription(Products.Activate.Description);

        productsGroup.MapPatch(Products.Deactivate.Pattern,
                async ([FromRoute] string productId, [FromQuery] Guid appUserId, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeactivateProductCommand(new AppUserId(appUserId), new ProductId(productId));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                    {
                        await cache.EvictByTagAsync(nameof(Products), ct);
                        return Results.Ok(product);
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces<ProductResponse>()
            .WithName(Products.Deactivate.Name)
            .WithDescription(Products.Deactivate.Description);

        productsGroup.MapDelete(Products.Delete.Pattern,
                async ([FromRoute] string productId, [FromQuery] Guid appUserId, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeleteProductCommand(new AppUserId(appUserId), new ProductId(productId));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async _ =>
                        {
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            return Results.NoContent();
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces(StatusCodes.Status204NoContent)
            .WithName(Products.Delete.Name)
            .WithDescription(Products.Delete.Description);
    }
}

