using Dashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<ServiceEndpoint> ServiceEndpoints { get; }
    DbSet<HealthCheckResult> HealthCheckResults { get; }
    DbSet<Incident> Incidents { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
