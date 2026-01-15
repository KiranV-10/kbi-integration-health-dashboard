using Dashboard.Domain.Enums;

namespace Dashboard.Application.Models;

public record IncidentDto(
    long Id,
    int ServiceEndpointId,
    string Title,
    IncidentSeverity Severity,
    IncidentStatus Status,
    string ReportedBy,
    DateTime ReportedAtUtc,
    string? Description,
    string? ResolutionNotes,
    DateTime? ResolvedAtUtc
);
