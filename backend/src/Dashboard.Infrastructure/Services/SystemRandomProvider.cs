using Dashboard.Application.Abstractions;

namespace Dashboard.Infrastructure.Services;

public class SystemRandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public int Next(int minValueInclusive, int maxValueExclusive)
    {
        return _random.Next(minValueInclusive, maxValueExclusive);
    }
}
