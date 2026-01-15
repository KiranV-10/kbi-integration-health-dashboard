using Dashboard.Domain.Enums;

namespace Dashboard.Domain.Entities;

public class HealthCheckResult
{
    public long Id { get; set; }
    public int ServiceEndpointId { get; set; }
    public DateTime CheckedAtUtc { get; set; }
    public HealthStatus Status { get; set; }
    public int? LatencyMs { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? ErrorMessage { get; set; }

    public ServiceEndpoint? ServiceEndpoint { get; set; }
}
