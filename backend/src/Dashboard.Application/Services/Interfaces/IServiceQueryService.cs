using Dashboard.Application.Models;

namespace Dashboard.Application.Services.Interfaces;

public interface IServiceQueryService
{
    Task<IReadOnlyList<ServiceSummaryDto>> GetServicesAsync(
        bool? activeOnly,
        string? search,
        CancellationToken cancellationToken);
}
