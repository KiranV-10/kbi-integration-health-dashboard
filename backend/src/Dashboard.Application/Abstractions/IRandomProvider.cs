namespace Dashboard.Application.Abstractions;

public interface IRandomProvider
{
    int Next(int minValueInclusive, int maxValueExclusive);
}
