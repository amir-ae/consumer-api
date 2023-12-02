using Consumer.Application.Common.Interfaces.Services;

namespace Consumer.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}