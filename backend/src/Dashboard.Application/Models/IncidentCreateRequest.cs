using Dashboard.Domain.Enums;

namespace Dashboard.Application.Models;

public class IncidentCreateRequest
{
    public int ServiceEndpointId { get; set; }
    public string Title { get; set; } = string.Empty;
    public IncidentSeverity Severity { get; set; }
    public string ReportedBy { get; set; } = string.Empty;
    public string? Description { get; set; }
}
