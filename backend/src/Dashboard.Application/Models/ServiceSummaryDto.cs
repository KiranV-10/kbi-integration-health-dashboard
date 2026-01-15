using Dashboard.Domain.Enums;

namespace Dashboard.Application.Models;

public record ServiceSummaryDto(
    int Id,
    string Name,
    string? Description,
    string BaseUrl,
    string? OwnerTeam,
    bool IsActive,
    DateTime? LastCheckedAtUtc,
    HealthStatus? LastStatus,
    int? LatencyMs,
    string? LastErrorMessage,
    int OpenIncidentCount
);
