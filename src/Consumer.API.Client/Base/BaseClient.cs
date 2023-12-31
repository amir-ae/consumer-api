using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Newtonsoft.Json;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Consumer.API.Client.Base;

public class BaseClient : IBaseClient
{
    private readonly string _baseUri;
    private readonly HttpClient _client;

    public BaseClient(HttpClient client, string baseUri)
    {
        _client = client;
        _baseUri = baseUri;
    }

    public async Task<ErrorOr<T>> Get<T>(Uri uri, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetAsync(uri, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> PostAsJson<T, U>(Uri uri, U data, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync(uri, data, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> PutAsJson<T, U>(Uri uri, U data, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.PutAsJsonAsync(uri, data, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }
    
    public async Task<ErrorOr<T>> PatchAsJson<T, U>(Uri uri, U data, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.PatchAsJsonAsync(uri, data, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> Patch<T>(Uri uri, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.PatchAsync(uri.OriginalString, null, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> Delete<T>(Uri uri, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.DeleteAsync(uri, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> ReadContentAs<T>(HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.IsSuccessStatusCode)
        {
            var dataAsString = await response.Content.ReadAsStringAsync(ct);
            return JsonConvert.DeserializeObject<T>(dataAsString)!;
        }

        return await ErrorContent<T>(response, ct);
    }

    public async Task<ErrorOr<T>> ErrorContent<T>(HttpResponseMessage response, CancellationToken ct = default)
    {
        string? description;
        try
        {
            var problemAsString = await response.Content.ReadAsStringAsync(ct);
            try
            {
                var problem = JsonConvert.DeserializeObject<ProblemDetails>(problemAsString)!;
                if (problem.Status == (int)HttpStatusCode.BadRequest)
                {
                    try
                    {
                        var validationProblem = JsonConvert.DeserializeObject<HttpValidationProblemDetails>(problemAsString)!;
                        var errors = validationProblem.Errors;
                        return errors.Select(kvp =>
                            Error.Validation(kvp.Key, kvp.Value.First())).ToList();
                    }
                    catch
                    {
                        // ignore
                    }
                }
                description = problem.Detail ?? problem.Title ?? string.Empty;
            }
            catch
            {
                description = problemAsString;
            }
        }
        catch
        {
            description = response.ReasonPhrase ?? string.Empty;
        }

        var statusCode = (int)response.StatusCode;
        switch (statusCode)
        {
            case 400:
            case 401:
                try
                {
                    var errors = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(description);
                    return errors!.Select(kvp => 
                        Error.Validation(kvp.Key, kvp.Value.First())).ToList();
                }
                catch
                {
                    return Error.Validation(statusCode.ToString(), description);
                }
            case 404:
                return Error.NotFound(statusCode.ToString(), description);
            case 409:
            case 499:
                return Error.Conflict(statusCode.ToString(), description);
            default:
                return Error.Failure(statusCode.ToString(), description);
        }
    }

    public Uri BuildUri(string path, Dictionary<string, string>? queries = default)
    {
        var uri = new UriBuilder(_baseUri)
        {
            Path = path
        }.Uri;

        if (queries?.Count > 0)
        {
            foreach (var kvp in queries)
            {
                uri = AddQuery(uri, kvp.Key, kvp.Value);
            }
        }
        return uri;
    }

    private Uri AddQuery(Uri uri, string name, string value)
    {
        var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

        httpValueCollection.Remove(name);
        httpValueCollection.Add(name, value);

        var ub = new UriBuilder(uri)
        {
            Query = httpValueCollection.ToString()
        };

        return ub.Uri;
    }
    
    private void AddAuthorizationHeader(string? jwt)
    {
        _client.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(jwt) ?
            null : new AuthenticationHeaderValue("bearer", jwt);
    }
}