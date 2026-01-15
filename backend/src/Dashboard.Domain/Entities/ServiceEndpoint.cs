namespace Dashboard.Domain.Entities;

public class ServiceEndpoint
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
    public string? OwnerTeam { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public ICollection<HealthCheckResult> HealthChecks { get; set; } = new List<HealthCheckResult>();
    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
}
