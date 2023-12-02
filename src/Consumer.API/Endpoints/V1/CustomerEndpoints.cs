using Consumer.API.Extensions.ErrorHandling;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Requests;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Application.Customers.Commands.Delete;
using Consumer.Application.Customers.Commands.Update;
using Consumer.Application.Customers.Commands.Activate;
using Consumer.Application.Customers.Commands.Deactivate;
using Consumer.Application.Customers.Queries.All;
using Consumer.Application.Customers.Queries.AllDetail;
using Consumer.Application.Customers.Queries.ByPage;
using Consumer.Application.Customers.Queries.ById;
using Consumer.Application.Customers.Queries.DetailById;
using Consumer.Application.Customers.Queries.EventsById;
using Consumer.Application.Customers.Queries.ByPageDetail;
using Consumer.Application.Customers.Commands.AddOrder;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers.ValueObjects;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Customers = Consumer.API.Contract.V1.Routes.Customers;
using Products = Consumer.API.Contract.V1.Routes.Products;

namespace Consumer.API.Endpoints.V1;

public static class CustomerEndpoints
{
    public static void AddCustomerEndpoints(this WebApplication app)
    {
        var customersGroup = app.MapGroup(Customers.Prefix)
            //.RequireAuthorization()
            //.AddEndpointFilter<ApiKeyEndpointFilter>()
            .WithTags(nameof(Customers))
            .WithOpenApi();

        customersGroup.MapGet(Customers.ByPage.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageIndex, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new CustomersByPageQuery(pageSize ?? 10, pageIndex ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new CustomerId(keyId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<PaginatedList<CustomerResponse>>()
            .WithName(Customers.ByPage.Name)
            .WithDescription(Customers.ByPage.Description)
            .CacheOutput("Auth");

        customersGroup.MapGet(Customers.ByPageDetail.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageIndex, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new CustomersByPageDetailQuery(pageSize ?? 10, pageIndex ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new CustomerId(keyId) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<PaginatedList<CustomerResponse>>()
            .WithName(Customers.ByPageDetail.Name)
            .WithDescription(Customers.ByPageDetail.Description)
            .CacheOutput("Auth");

        customersGroup.MapGet(Customers.All.Pattern,
                async (ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new AllCustomersQuery();
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<List<CustomerForListingResponse>>()
            .WithName(Customers.All.Name)
            .WithDescription(Customers.All.Description)
            .CacheOutput("Auth");
        
        customersGroup.MapGet(Customers.AllDetail.Pattern, 
                async (ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new AllCustomersDetailQuery();
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<List<CustomerResponse>>()
            .WithName(Customers.AllDetail.Name)
            .WithDescription(Customers.AllDetail.Description)
            .CacheOutput("Auth");
        
        customersGroup.MapGet(Customers.ById.Pattern,
                async ([FromRoute] string customerId, ISender mediator, IErrorHandler errorHandler,
                    CancellationToken ct) =>
                {
                    var query = new CustomerByIdQuery(new CustomerId(customerId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<CustomerResponse>()
            .WithName(Customers.ById.Name)
            .WithDescription(Customers.ById.Description);

        customersGroup.MapGet(Customers.DetailById.Pattern,
                async ([FromRoute] string customerId, ISender mediator, IErrorHandler errorHandler, 
                    CancellationToken ct) =>
                {
                    var query = new CustomerDetailByIdQuery(new CustomerId(customerId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<CustomerResponse>()
            .WithName(Customers.DetailById.Name)
            .WithDescription(Customers.DetailById.Description);

        customersGroup.MapGet(Customers.EventsById.Pattern,
                async ([FromRoute] string customerId, ISender mediator, IErrorHandler errorHandler, 
                    CancellationToken ct) =>
                {
                    var query = new CustomerEventsByIdQuery(new CustomerId(customerId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                }).Produces<CustomerEventsResponse>()
            .WithName(Customers.EventsById.Name)
            .WithDescription(Customers.EventsById.Description);

        customersGroup.MapPost(Customers.Post.Pattern,
                async ([FromBody] PostCustomerRequest request, ISender mediator, IOutputCacheStore cache, 
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<CreateCustomerCommand>();
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                        {
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            return Results.Created(new Uri(Customers.ById.Uri(customer.CustomerId), 
                                UriKind.Relative), customer);
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces<CustomerResponse>(StatusCodes.Status201Created)
            .WithName(Customers.Post.Name)
            .WithDescription(Customers.Post.Description);
        
        customersGroup.MapPost(Customers.PostOrder.Pattern,
                async ([FromBody] PostCustomerOrderRequest request, ISender mediator, IOutputCacheStore cache, 
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<AddOrderCommand>();
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async orderResult =>
                        {
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            return Results.Ok(orderResult);
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces<CustomerOrderResponse>()
            .WithName(Customers.PostOrder.Name)
            .WithDescription(Customers.PostOrder.Description);
        
        customersGroup.MapPatch(Customers.Patch.Pattern,
                async ([FromRoute] string customerId, [FromBody] PatchCustomerRequest request,
                    ISender mediator, IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<UpdateCustomerCommand>() with { CustomerId = new CustomerId(customerId) };
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                        {
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            return Results.Ok(customer);
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces<CustomerResponse>()
            .WithName(Customers.Patch.Name)
            .WithDescription(Customers.Patch.Description);

        customersGroup.MapPatch(Customers.Activate.Pattern,
                async ([FromRoute] string customerId, [FromQuery] Guid appUserId, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new ActivateCustomerCommand(new AppUserId(appUserId), new CustomerId(customerId));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                    {
                        await cache.EvictByTagAsync(nameof(Customers), ct);
                        return Results.Ok(customer);
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces<CustomerResponse>()
            .WithName(Customers.Activate.Name)
            .WithDescription(Customers.Activate.Description);

        customersGroup.MapPatch(Customers.Deactivate.Pattern,
                async ([FromRoute] string customerId, [FromQuery] Guid appUserId, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeactivateCustomerCommand(new AppUserId(appUserId), new CustomerId(customerId));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                    {
                        await cache.EvictByTagAsync(nameof(Customers), ct);
                        return Results.Ok(customer);
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces<CustomerResponse>()
            .WithName(Customers.Deactivate.Name)
            .WithDescription(Customers.Deactivate.Description);

        customersGroup.MapDelete(Customers.Delete.Pattern,
                async ([FromRoute] string customerId, [FromQuery] Guid appUserId, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeleteCustomerCommand(new AppUserId(appUserId), new CustomerId(customerId));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async _ =>
                        {
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            return Results.NoContent();
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                }).Produces(StatusCodes.Status204NoContent)
            .WithName(Customers.Delete.Name)
            .WithDescription(Customers.Delete.Description);
    }
}