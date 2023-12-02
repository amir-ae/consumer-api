using Microsoft.Net.Http.Headers;

namespace Consumer.API.Extensions.ETag;

public static class ETagExtensions
{
    public static int? ToExpectedVersion(string? eTag)
    {
        if (string.IsNullOrWhiteSpace(eTag)) return null;

        var value = EntityTagHeaderValue.Parse(eTag).Tag.Value;

        if (string.IsNullOrWhiteSpace(value)) return null;

        if (int.TryParse(value.Substring(1, value.Length - 2), out int result))
        {
            return result;
        }

        return null;
    }
    
    public static string? ToHash(string? eTag)
    {
        if (string.IsNullOrWhiteSpace(eTag)) return null;

        return EntityTagHeaderValue.Parse(eTag).Tag.Value;
    }
}