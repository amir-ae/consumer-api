using Consumer.API.Extensions.ErrorHandling;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Requests;
using Consumer.API.Contract.V1.Products.Responses;
using Consumer.API.Extensions.ETag;
using Consumer.Application.Products.Commands.Create;
using Consumer.Application.Products.Commands.Delete;
using Consumer.Application.Products.Commands.Update;
using Consumer.Application.Products.Commands.Activate;
using Consumer.Application.Products.Commands.Deactivate;
using Consumer.Application.Products.Queries.List;
using Consumer.Application.Products.Queries.ListDetail;
using Consumer.Application.Products.Queries.ByPage;
using Consumer.Application.Products.Queries.ById;
using Consumer.Application.Products.Queries.DetailById;
using Consumer.Application.Products.Queries.DetailByOrderId;
using Consumer.Application.Products.Queries.EventsById;
using Consumer.Application.Products.Queries.ByPageDetail;
using Consumer.Application.Products.Queries.CheckById;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Products;
using Consumer.Domain.Products.ValueObjects;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Products = Consumer.API.Contract.V1.Routes.Products;
using Customers = Consumer.API.Contract.V1.Routes.Customers;
using static Consumer.API.Extensions.ETag.ETagExtensions;

namespace Consumer.API.Endpoints.V1;

public static class ProductEndpoints
{
    public static void AddProductEndpoints(this WebApplication app)
    {
        var productsGroup = app.MapGroup(Products.Prefix)
            //.RequireAuthorization()
            //.AddEndpointFilter<ApiKeyEndpointFilter>()
            .AddEndpointFilter<ETagEndpointFilter<ProductResponse, ProductForListingResponse>>()
            .WithTags(nameof(Product))
            .WithOpenApi();

        productsGroup.MapGet(Products.ByPage.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageIndex, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, [FromQuery] Guid? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductsByPageQuery(pageSize ?? 10, pageIndex ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new ProductId(keyId) : null,
                        centreId.HasValue ? new CentreId(centreId.Value) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<PaginatedList<ProductResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.ByPage.Name)
            .WithDescription(Products.ByPage.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.ByPageDetail.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageIndex, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, [FromQuery] Guid? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductsByPageDetailQuery(pageSize ?? 10, pageIndex ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new ProductId(keyId) : null,
                        centreId.HasValue ? new CentreId(centreId.Value) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<PaginatedList<ProductResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.ByPageDetail.Name)
            .WithDescription(Products.ByPageDetail.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.List.Pattern,
                async ([FromQuery] Guid? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ListProductsQuery(centreId.HasValue ? new CentreId(centreId.Value) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<List<ProductForListingResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.List.Name)
            .WithDescription(Products.List.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.ListDetail.Pattern,
                async ([FromQuery] Guid? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ListProductsDetailQuery(centreId.HasValue ? new CentreId(centreId.Value) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<List<ProductResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.ListDetail.Name)
            .WithDescription(Products.ListDetail.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.ById.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<ProductResponse>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.ById.Name)
            .WithDescription(Products.ById.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.DetailById.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductDetailByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<ProductResponse>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.DetailById.Name)
            .WithDescription(Products.DetailById.Description)
            .CacheOutput("Auth");

        productsGroup.MapGet(Products.EventsById.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ProductEventsByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<ProductEventsResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.EventsById.Name)
            .WithDescription(Products.EventsById.Description);
        
        productsGroup.MapGet(Products.Check.Pattern,
                async ([FromRoute] string productId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new CheckProductByIdQuery(new ProductId(productId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<bool>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Check.Name)
            .WithDescription(Products.Check.Description);

        productsGroup.MapGet(Products.DetailByOrderId.Pattern,
                async ([FromRoute] string orderId, ISender mediator, IErrorHandler errorHandler,
                    CancellationToken ct) =>
                {
                    var query = new ProductDetailByOrderIdQuery(new OrderId(orderId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<ProductResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.DetailByOrderId.Name)
            .WithDescription(Products.DetailByOrderId.Description);

        productsGroup.MapPost(Products.Create.Pattern,
                async ([FromBody] CreateProductRequest request, ISender mediator, IOutputCacheStore cache,
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<CreateProductCommand>();
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                        {
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            return Results.Created(new Uri(Products.ById.Uri(product.Id.Value), UriKind.Relative),
                                product.Adapt<ProductResponse>());
                        }, 
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<ProductResponse>(StatusCodes.Status201Created)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Create.Name)
            .WithDescription(Products.Create.Description);

        productsGroup.MapPatch(Products.Update.Pattern,
                async ([FromRoute] string productId, [FromHeader(Name = "If-Match")] string? eTag, 
                    [FromBody] UpdateProductRequest request, ISender mediator, IOutputCacheStore cache, 
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<UpdateProductCommand>() with
                    {
                        ProductId = new ProductId(productId),
                        Version = ToExpectedVersion(eTag)
                    };
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                        {
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            return Results.Ok(product.Adapt<ProductResponse>());
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<ProductResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Update.Name)
            .WithDescription(Products.Update.Description);

        productsGroup.MapPatch(Products.Activate.Pattern,
                async ([FromRoute] string productId, [FromQuery] Guid activateBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new ActivateProductCommand(new ProductId(productId), new AppUserId(activateBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                    {
                        await cache.EvictByTagAsync(nameof(Products), ct);
                        return Results.Ok(product.Adapt<ProductResponse>());
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<ProductResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Activate.Name)
            .WithDescription(Products.Activate.Description);

        productsGroup.MapPatch(Products.Deactivate.Pattern,
                async ([FromRoute] string productId, [FromQuery] Guid deactivateBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeactivateProductCommand(new ProductId(productId), new AppUserId(deactivateBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async product =>
                    {
                        await cache.EvictByTagAsync(nameof(Products), ct);
                        return Results.Ok(product.Adapt<ProductResponse>());
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<ProductResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Deactivate.Name)
            .WithDescription(Products.Deactivate.Description);

        productsGroup.MapDelete(Products.Delete.Pattern,
                async ([FromRoute] string productId, [FromQuery] Guid deleteBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeleteProductCommand(new ProductId(productId), new AppUserId(deleteBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async _ =>
                        {
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            return Results.NoContent();
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Products.Delete.Name)
            .WithDescription(Products.Delete.Description);
    }
}

