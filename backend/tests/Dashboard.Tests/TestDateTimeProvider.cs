using Dashboard.Application.Abstractions;

namespace Dashboard.Tests;

public class TestDateTimeProvider : IDateTimeProvider
{
    public TestDateTimeProvider(DateTime utcNow)
    {
        UtcNow = utcNow;
    }

    public DateTime UtcNow { get; set; }
}
