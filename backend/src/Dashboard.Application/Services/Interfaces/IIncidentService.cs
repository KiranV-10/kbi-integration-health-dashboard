using Dashboard.Application.Models;
using Dashboard.Domain.Enums;

namespace Dashboard.Application.Services.Interfaces;

public interface IIncidentService
{
    Task<IReadOnlyList<IncidentDto>> GetIncidentsAsync(
        IncidentStatus? status,
        CancellationToken cancellationToken);

    Task<IncidentDto> CreateIncidentAsync(
        IncidentCreateRequest request,
        CancellationToken cancellationToken);

    Task<IncidentDto?> UpdateStatusAsync(
        long id,
        IncidentStatusUpdateRequest request,
        CancellationToken cancellationToken);

    Task<IncidentDto?> ResolveAsync(
        long id,
        IncidentResolveRequest request,
        CancellationToken cancellationToken);
}
