﻿using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Products.Responses;
using MediatR;
using ErrorOr;
using Consumer.Application.Common.Interfaces.Persistence;
using Mapster;

namespace Consumer.Application.Products.Queries.ByPage;

public sealed class ProductsByPageQueryHandler : IRequestHandler<ProductsByPageQuery, ErrorOr<PaginatedList<ProductResponse>>>
{
    private readonly IProductRepository _productRepository;

    public ProductsByPageQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ErrorOr<PaginatedList<ProductResponse>>> Handle(ProductsByPageQuery query, CancellationToken ct = default)
    {
        var (pageSize, pageNumber, nextPage, keyId, centreId) = query;

        var (products, totalCount) = await _productRepository.ByPageAsync(pageSize, pageNumber, nextPage, keyId, centreId, ct);

        var result = products.Adapt<List<ProductResponse>>();

        return new PaginatedList<ProductResponse>(
            pageNumber, pageSize, totalCount, result);
    }
}