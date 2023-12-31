﻿using Consumer.API.Contract.V1.Common.Responses;
using Consumer.API.Contract.V1.Customers.Responses;
using Consumer.Domain.Customers.ValueObjects;
using ErrorOr;
using MediatR;

namespace Consumer.Application.Customers.Queries.ByPage;

public record CustomersByPageQuery(
    int PageSize,
    int PageIndex,
    bool? NextPage,
    CustomerId? KeyId) : IRequest<ErrorOr<PaginatedList<CustomerResponse>>>;