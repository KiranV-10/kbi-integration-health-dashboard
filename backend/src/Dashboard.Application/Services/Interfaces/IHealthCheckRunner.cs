using Dashboard.Application.Models;

namespace Dashboard.Application.Services.Interfaces;

public interface IHealthCheckRunner
{
    Task<IReadOnlyList<HealthCheckResultDto>> RunChecksAsync(CancellationToken cancellationToken);
}
