namespace Dashboard.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
