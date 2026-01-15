using Dashboard.Application.Abstractions;
using Dashboard.Application.Models;
using Dashboard.Application.Services.Interfaces;
using Dashboard.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Application.Services;

public class ServiceQueryService : IServiceQueryService
{
    private readonly IAppDbContext _db;

    public ServiceQueryService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<ServiceSummaryDto>> GetServicesAsync(
        bool? activeOnly,
        string? search,
        CancellationToken cancellationToken)
    {
        var query = _db.ServiceEndpoints.AsNoTracking();

        if (activeOnly == true)
        {
            query = query.Where(service => service.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(service => service.Name.Contains(search));
        }

        var results = await query
            .OrderBy(service => service.Name)
            .Select(service => new ServiceSummaryDto(
                service.Id,
                service.Name,
                service.Description,
                service.BaseUrl,
                service.OwnerTeam,
                service.IsActive,
                _db.HealthCheckResults
                    .Where(check => check.ServiceEndpointId == service.Id)
                    .OrderByDescending(check => check.CheckedAtUtc)
                    .Select(check => (DateTime?)check.CheckedAtUtc)
                    .FirstOrDefault(),
                _db.HealthCheckResults
                    .Where(check => check.ServiceEndpointId == service.Id)
                    .OrderByDescending(check => check.CheckedAtUtc)
                    .Select(check => (HealthStatus?)check.Status)
                    .FirstOrDefault(),
                _db.HealthCheckResults
                    .Where(check => check.ServiceEndpointId == service.Id)
                    .OrderByDescending(check => check.CheckedAtUtc)
                    .Select(check => check.LatencyMs)
                    .FirstOrDefault(),
                _db.HealthCheckResults
                    .Where(check => check.ServiceEndpointId == service.Id)
                    .OrderByDescending(check => check.CheckedAtUtc)
                    .Select(check => check.ErrorMessage)
                    .FirstOrDefault(),
                service.Incidents.Count(incident => incident.Status != IncidentStatus.Resolved)))
            .ToListAsync(cancellationToken);

        return results;
    }
}
