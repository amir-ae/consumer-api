﻿using Commerce.API.Contract.V1.Common.Models;
using Commerce.API.Contract.V1.Common.Responses;
using Commerce.API.Contract.V1.Customers.Responses.Models;

namespace Commerce.API.Contract.V1.Customers.Responses;

public record CustomerBriefResponse(string Id,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    PhoneNumber PhoneNumber,
    City City,
    string Address,
    CustomerRole Role,
    IList<string> ProductIds,
    IList<OrderId> OrderIds,
    DateTimeOffset CreatedAt) : BriefResponse(CreatedAt);