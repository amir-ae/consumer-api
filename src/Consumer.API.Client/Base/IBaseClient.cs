using ErrorOr;

namespace Consumer.API.Client.Base;

public interface IBaseClient
{
    Task<ErrorOr<T>> Get<T>(Uri uri, CancellationToken ct = default);
    Task<ErrorOr<T>> PostAsJson<T, U>(Uri uri, U data, CancellationToken ct = default);
    Task<ErrorOr<T>> PutAsJson<T, U>(Uri uri, U data, CancellationToken ct = default);
    Task<ErrorOr<T>> PatchAsJson<T, U>(Uri uri, U data, CancellationToken ct = default);
    Task<ErrorOr<T>> Patch<T>(Uri uri, CancellationToken ct = default);
    Task<ErrorOr<T>> Delete<T>(Uri uri, CancellationToken ct = default);
    Uri BuildUri(string path, Dictionary<string, string>? queries = default);
}