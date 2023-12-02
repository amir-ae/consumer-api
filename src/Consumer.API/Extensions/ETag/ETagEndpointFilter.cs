using System.Security.Cryptography;
using System.Text;
using Consumer.API.Contract.V1.Common.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using static Consumer.API.Extensions.ETag.ETagExtensions;

namespace Consumer.API.Extensions.ETag;

public class ETagEndpointFilter<T, TU> : IEndpointFilter 
    where T : AuditableResponse 
    where TU : ForListingResponse
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var request = context.HttpContext.Request;
        
        var response = await next(context);
        
        if (request.Method != HttpMethod.Get.Method)
        {
            return response;
        }

        var hash = response switch
        {
            Ok<PaginatedList<T>> paginatedList => Hash(paginatedList.Value),
            Ok<T[]> list => Hash(list.Value),
            Ok<List<T>> list => Hash(list.Value),
            Ok<TU[]> list => Hash(list.Value),
            Ok<List<TU>> list => Hash(list.Value),
            Ok<T> ok => Hash(ok.Value),
            _ => string.Empty
        };

        if (string.IsNullOrWhiteSpace(hash))
        {
            return response;
        }
        
        var eTag = new EntityTagHeaderValue('\"' + hash + '\"');
        context.HttpContext.Response.Headers.TryAdd(HeaderNames.ETag, eTag.ToString());
            
        if (request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var ifNoneMatch))
        {
            var headerValue = ToHash(ifNoneMatch);
            if (!string.IsNullOrEmpty(headerValue) && headerValue.Equals(hash))
            {
                return new StatusCodeResult(StatusCodes.Status304NotModified);
            }
        }

        return response;
    }

    private string Hash(T? body)
    {
        if (body is null) return string.Empty;
        var str = new { 
            body.CreatedAt, body.CreatedBy, body.LastModifiedAt, body.LastModifiedBy
        }.ToString();
        if (string.IsNullOrWhiteSpace(str)) return string.Empty;
        return GetHashString(str);
    }
    
    private string Hash(IEnumerable<T>? body)
    {
        if (body is null) return string.Empty;
        var list = body.Select(item 
            => new { item.CreatedAt, item.CreatedBy, item.LastModifiedAt, item.LastModifiedBy });
        return GetHashString(string.Join( ",", list));
    }
    
    private string Hash(IEnumerable<TU>? body)
    {
        if (body is null) return string.Empty;
        var list = body.Select(item => item.ToString());
        return GetHashString(string.Join( ",", list));
    }
    
    private string Hash(PaginatedList<T>? body)
    {
        if (body is null) return string.Empty;
        var list = body.Data.Select(item 
            => new { item.CreatedAt, item.CreatedBy, item.LastModifiedAt, item.LastModifiedBy });
        return GetHashString(string.Join( ",", list));
    }

    private static string GetHashString(string inputString)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHash(inputString))
        {
            sb.Append(b.ToString("X2"));
        }
        
        return sb.ToString();
    }
    
    private static byte[] GetHash(string inputString)
    {
        using HashAlgorithm algorithm = SHA256.Create();
        return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }
}