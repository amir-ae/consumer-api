namespace Consumer.Infrastructure.Extensions;

public static class StringExtensions
{
    public static string AppendTimeStamp(this string fileName)
    {
        return string.Concat(
            Path.GetDirectoryName(fileName),
            "\\",
            Path.GetFileNameWithoutExtension(fileName),
            DateTime.Now.ToString("yyyyMMddHHmmssfff"),
            Path.GetExtension(fileName)
        );
    }
}