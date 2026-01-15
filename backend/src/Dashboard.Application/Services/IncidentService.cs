using Dashboard.Application.Abstractions;
using Dashboard.Application.Models;
using Dashboard.Application.Services.Interfaces;
using Dashboard.Domain.Entities;
using Dashboard.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Application.Services;

public class IncidentService : IIncidentService
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _clock;

    public IncidentService(IAppDbContext db, IDateTimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<IReadOnlyList<IncidentDto>> GetIncidentsAsync(
        IncidentStatus? status,
        CancellationToken cancellationToken)
    {
        var query = _db.Incidents.AsNoTracking();

        if (status.HasValue)
        {
            query = query.Where(incident => incident.Status == status);
        }

        var incidents = await query
            .OrderByDescending(incident => incident.ReportedAtUtc)
            .Select(incident => new IncidentDto(
                incident.Id,
                incident.ServiceEndpointId,
                incident.Title,
                incident.Severity,
                incident.Status,
                incident.ReportedBy,
                incident.ReportedAtUtc,
                incident.Description,
                incident.ResolutionNotes,
                incident.ResolvedAtUtc))
            .ToListAsync(cancellationToken);

        return incidents;
    }

    public async Task<IncidentDto> CreateIncidentAsync(
        IncidentCreateRequest request,
        CancellationToken cancellationToken)
    {
        ValidateCreateRequest(request);

        var serviceExists = await _db.ServiceEndpoints
            .AsNoTracking()
            .AnyAsync(service => service.Id == request.ServiceEndpointId, cancellationToken);

        if (!serviceExists)
        {
            throw new InvalidOperationException("Service endpoint not found.");
        }

        var incident = new Incident
        {
            ServiceEndpointId = request.ServiceEndpointId,
            Title = request.Title.Trim(),
            Severity = request.Severity,
            Status = IncidentStatus.Open,
            ReportedBy = request.ReportedBy.Trim(),
            ReportedAtUtc = _clock.UtcNow,
            Description = request.Description?.Trim()
        };

        _db.Incidents.Add(incident);
        await _db.SaveChangesAsync(cancellationToken);

        return MapIncident(incident);
    }

    public async Task<IncidentDto?> UpdateStatusAsync(
        long id,
        IncidentStatusUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var incident = await _db.Incidents
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (incident is null)
        {
            return null;
        }

        incident.Status = request.Status;

        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            incident.ResolutionNotes = request.Notes.Trim();
        }

        if (request.Status == IncidentStatus.Resolved && incident.ResolvedAtUtc is null)
        {
            incident.ResolvedAtUtc = _clock.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return MapIncident(incident);
    }

    public async Task<IncidentDto?> ResolveAsync(
        long id,
        IncidentResolveRequest request,
        CancellationToken cancellationToken)
    {
        var incident = await _db.Incidents
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (incident is null)
        {
            return null;
        }

        incident.Status = IncidentStatus.Resolved;
        incident.ResolutionNotes = request.ResolutionNotes?.Trim();
        incident.ResolvedAtUtc = _clock.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return MapIncident(incident);
    }

    private static IncidentDto MapIncident(Incident incident)
    {
        return new IncidentDto(
            incident.Id,
            incident.ServiceEndpointId,
            incident.Title,
            incident.Severity,
            incident.Status,
            incident.ReportedBy,
            incident.ReportedAtUtc,
            incident.Description,
            incident.ResolutionNotes,
            incident.ResolvedAtUtc);
    }

    private static void ValidateCreateRequest(IncidentCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Incident title is required.");
        }

        if (string.IsNullOrWhiteSpace(request.ReportedBy))
        {
            throw new ArgumentException("Reported by is required.");
        }
    }
}
