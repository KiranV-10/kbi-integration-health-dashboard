using Dashboard.Domain.Enums;

namespace Dashboard.Application.Models;

public record HealthCheckResultDto(
    int ServiceEndpointId,
    DateTime CheckedAtUtc,
    HealthStatus Status,
    int? LatencyMs,
    int? HttpStatusCode,
    string? ErrorMessage
);
