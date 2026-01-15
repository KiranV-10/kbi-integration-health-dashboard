using Dashboard.Application.Services;
using Dashboard.Domain.Enums;
using Xunit;

namespace Dashboard.Tests;

public class HealthStatusEvaluatorTests
{
    [Fact]
    public void Evaluate_ReturnsOk_ForFastSuccessfulRequest()
    {
        var status = HealthStatusEvaluator.Evaluate(true, 200, 120, hasException: false);

        Assert.Equal(HealthStatus.Ok, status);
    }

    [Fact]
    public void Evaluate_ReturnsDegraded_ForSlowSuccess()
    {
        var status = HealthStatusEvaluator.Evaluate(true, 200, 700, hasException: false);

        Assert.Equal(HealthStatus.Degraded, status);
    }

    [Theory]
    [InlineData(429)]
    [InlineData(503)]
    public void Evaluate_ReturnsDegraded_ForThrottledOrUnavailable(int statusCode)
    {
        var status = HealthStatusEvaluator.Evaluate(false, statusCode, 120, hasException: false);

        Assert.Equal(HealthStatus.Degraded, status);
    }

    [Fact]
    public void Evaluate_ReturnsDown_ForServerError()
    {
        var status = HealthStatusEvaluator.Evaluate(false, 500, 120, hasException: false);

        Assert.Equal(HealthStatus.Down, status);
    }
}
