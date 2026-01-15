using Dashboard.Domain.Enums;

namespace Dashboard.Domain.Entities;

public class Incident
{
    public long Id { get; set; }
    public int ServiceEndpointId { get; set; }
    public string Title { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; }
    public IncidentStatus Status { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public DateTime ReportedAtUtc { get; set; }
    public string? Description { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }

    public ServiceEndpoint? ServiceEndpoint { get; set; }
}
