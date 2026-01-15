using Dashboard.Domain.Enums;

namespace Dashboard.Application.Services;

public static class HealthStatusEvaluator
{
    public static HealthStatus Evaluate(bool isSuccess, int statusCode, int latencyMs, bool hasException)
    {
        if (hasException)
        {
            return HealthStatus.Down;
        }

        if (statusCode == 429 || statusCode == 503)
        {
            return HealthStatus.Degraded;
        }

        if (statusCode >= 500)
        {
            return HealthStatus.Down;
        }

        if (isSuccess && latencyMs < 500)
        {
            return HealthStatus.Ok;
        }

        if (isSuccess)
        {
            return HealthStatus.Degraded;
        }

        return HealthStatus.Down;
    }
}
