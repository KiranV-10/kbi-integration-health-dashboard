using Dashboard.Domain.Enums;

namespace Dashboard.Application.Models;

public class IncidentStatusUpdateRequest
{
    public IncidentStatus Status { get; set; }
    public string? Notes { get; set; }
}
