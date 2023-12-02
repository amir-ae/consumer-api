using Consumer.API.Extensions.ErrorHandling;
using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Requests;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.API.Extensions.ETag;
using Consumer.Application.Customers.Commands.Create;
using Consumer.Application.Customers.Commands.Delete;
using Consumer.Application.Customers.Commands.Update;
using Consumer.Application.Customers.Commands.Activate;
using Consumer.Application.Customers.Commands.Deactivate;
using Consumer.Application.Customers.Queries.List;
using Consumer.Application.Customers.Queries.ListDetail;
using Consumer.Application.Customers.Queries.ByPage;
using Consumer.Application.Customers.Queries.ById;
using Consumer.Application.Customers.Queries.DetailById;
using Consumer.Application.Customers.Queries.EventsById;
using Consumer.Application.Customers.Queries.ByPageDetail;
using Consumer.Domain.Common.ValueObjects;
using Consumer.Domain.Customers;
using Consumer.Domain.Customers.ValueObjects;
using Consumer.Domain.Products.ValueObjects;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Customers = Consumer.API.Contract.V1.Routes.Customers;
using Products = Consumer.API.Contract.V1.Routes.Products;
using static Consumer.API.Extensions.ETag.ETagExtensions;

namespace Consumer.API.Endpoints.V1;

public static class CustomerEndpoints
{
    public static void AddCustomerEndpoints(this WebApplication app)
    {
        var customersGroup = app.MapGroup(Customers.Prefix)
            
            //.RequireAuthorization()
            //.AddEndpointFilter<ApiKeyEndpointFilter>()
            .AddEndpointFilter<ETagEndpointFilter<CustomerResponse, CustomerForListingResponse>>()
            .WithTags(nameof(Customer))
            .WithOpenApi();

        customersGroup.MapGet(Customers.ByPage.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageIndex, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, [FromQuery] Guid? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new CustomersByPageQuery(pageSize ?? 10, pageIndex ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new CustomerId(keyId) : null,
                        centreId.HasValue ? new CentreId(centreId.Value) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<PaginatedList<CustomerResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.ByPage.Name)
            .WithDescription(Customers.ByPage.Description)
            .CacheOutput("Auth");

        customersGroup.MapGet(Customers.ByPageDetail.Pattern,
                async ([FromQuery] int? pageSize, [FromQuery] int? pageIndex, [FromQuery] bool? nextPage, 
                    [FromQuery] string? keyId, [FromQuery] Guid? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new CustomersByPageDetailQuery(pageSize ?? 10, pageIndex ?? 1, nextPage, 
                        !string.IsNullOrWhiteSpace(keyId) ? new CustomerId(keyId) : null,
                        centreId.HasValue ? new CentreId(centreId.Value) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<PaginatedList<CustomerResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.ByPageDetail.Name)
            .WithDescription(Customers.ByPageDetail.Description)
            .CacheOutput("Auth");

        customersGroup.MapGet(Customers.List.Pattern,
                async ([FromQuery] Guid? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ListCustomersQuery(centreId.HasValue ? new CentreId(centreId.Value) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<List<CustomerForListingResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.List.Name)
            .WithDescription(Customers.List.Description)
            .CacheOutput("Auth");
        
        customersGroup.MapGet(Customers.ListDetail.Pattern, 
                async ([FromQuery] Guid? centreId, ISender mediator, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var query = new ListCustomersDetailQuery(centreId.HasValue ? new CentreId(centreId.Value) : null);
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<List<CustomerResponse>>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.ListDetail.Name)
            .WithDescription(Customers.ListDetail.Description)
            .CacheOutput("Auth");
        
        customersGroup.MapGet(Customers.ById.Pattern,
                async ([FromRoute] string customerId, ISender mediator, IErrorHandler errorHandler,
                    CancellationToken ct) =>
                {
                    var query = new CustomerByIdQuery(new CustomerId(customerId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.ById.Name)
            .WithDescription(Customers.ById.Description);

        customersGroup.MapGet(Customers.DetailById.Pattern,
                async ([FromRoute] string customerId, ISender mediator, IErrorHandler errorHandler, 
                    CancellationToken ct) =>
                {
                    var query = new CustomerDetailByIdQuery(new CustomerId(customerId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<CustomerResponse>()
            .Produces(StatusCodes.Status304NotModified)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.DetailById.Name)
            .WithDescription(Customers.DetailById.Description);

        customersGroup.MapGet(Customers.EventsById.Pattern,
                async ([FromRoute] string customerId, ISender mediator, IErrorHandler errorHandler, 
                    CancellationToken ct) =>
                {
                    var query = new CustomerEventsByIdQuery(new CustomerId(customerId));
                    var result = await mediator.Send(query, ct).DefaultIfCanceled();
                    return result.Match(Results.Ok, errorHandler.Problem);
                })
            .Produces<CustomerEventsResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.EventsById.Name)
            .WithDescription(Customers.EventsById.Description);

        customersGroup.MapPost(Customers.Create.Pattern,
                async ([FromBody] CreateCustomerRequest request, ISender mediator, IOutputCacheStore cache, 
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<CreateCustomerCommand>();
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                        {
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            return Results.Created(new Uri(Customers.ById.Uri(customer.Id.Value), 
                                UriKind.Relative), customer.Adapt<CustomerResponse>());
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<CustomerResponse>(StatusCodes.Status201Created)
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.Create.Name)
            .WithDescription(Customers.Create.Description);

        customersGroup.MapPatch(Customers.Update.Pattern,
                async ([FromRoute] string customerId, [FromHeader(Name = "If-Match")] string? eTag,
                    [FromBody] UpdateCustomerRequest request, ISender mediator, IOutputCacheStore cache, 
                    IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = request.Adapt<UpdateCustomerCommand>() with
                    {
                        CustomerId = new CustomerId(customerId),
                        Version = ToExpectedVersion(eTag)
                    };
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                        {
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
                            return Results.Ok(customer.Adapt<CustomerResponse>());
                        },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<CustomerResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.Update.Name)
            .WithDescription(Customers.Update.Description);

        customersGroup.MapPatch(Customers.Activate.Pattern,
                async ([FromRoute] string customerId, [FromQuery] Guid activateBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new ActivateCustomerCommand(new CustomerId(customerId), new AppUserId(activateBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                    {
                        await cache.EvictByTagAsync(nameof(Customers), ct);
                        return Results.Ok(customer.Adapt<CustomerResponse>());
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<CustomerResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.Activate.Name)
            .WithDescription(Customers.Activate.Description);

        customersGroup.MapPatch(Customers.Deactivate.Pattern,
                async ([FromRoute] string customerId, [FromQuery] Guid deactivateBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeactivateCustomerCommand(new CustomerId(customerId), new AppUserId(deactivateBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async customer =>
                    {
                        await cache.EvictByTagAsync(nameof(Customers), ct);
                        return Results.Ok(customer.Adapt<CustomerResponse>());
                    },
                        errors => Task.FromResult(errorHandler.Problem(errors)));
                })
            .Produces<CustomerResponse>()
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status499ClientClosedRequest)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithName(Customers.Deactivate.Name)
            .WithDescription(Customers.Deactivate.Description);

        customersGroup.MapDelete(Customers.Delete.Pattern,
                async ([FromRoute] string customerId, [FromQuery] Guid deleteBy, ISender mediator,
                    IOutputCacheStore cache, IErrorHandler errorHandler, CancellationToken ct) =>
                {
                    var command = new DeleteCustomerCommand(new CustomerId(customerId), new AppUserId(deleteBy));
                    var result = await mediator.Send(command, ct).DefaultIfCanceled();
                    return await result.MatchAsync(async _ =>
                        {
                            await cache.EvictByTagAsync(nameof(Customers), ct);
                            await cache.EvictByTagAsync(nameof(Products), ct);
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
            .WithName(Customers.Delete.Name)
            .WithDescription(Customers.Delete.Description);
    }
}