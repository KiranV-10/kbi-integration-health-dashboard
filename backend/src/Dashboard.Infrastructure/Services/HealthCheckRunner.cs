using System.Diagnostics;
using Dashboard.Application.Abstractions;
using Dashboard.Application.Models;
using Dashboard.Application.Services;
using Dashboard.Application.Services.Interfaces;
using Dashboard.Domain.Entities;
using Dashboard.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Infrastructure.Services;

public class HealthCheckRunner : IHealthCheckRunner
{
    private readonly HttpClient _httpClient;
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _clock;
    private readonly IRandomProvider _random;

    public HealthCheckRunner(
        HttpClient httpClient,
        IAppDbContext db,
        IDateTimeProvider clock,
        IRandomProvider random)
    {
        _httpClient = httpClient;
        _db = db;
        _clock = clock;
        _random = random;
    }

    public async Task<IReadOnlyList<HealthCheckResultDto>> RunChecksAsync(CancellationToken cancellationToken)
    {
        var services = await _db.ServiceEndpoints
            .AsNoTracking()
            .Where(service => service.IsActive)
            .ToListAsync(cancellationToken);

        var results = new List<HealthCheckResult>();

        foreach (var service in services)
        {
            var result = await CheckServiceAsync(service, cancellationToken);
            results.Add(result);
        }

        _db.HealthCheckResults.AddRange(results);
        await _db.SaveChangesAsync(cancellationToken);

        return results.Select(result => new HealthCheckResultDto(
            result.ServiceEndpointId,
            result.CheckedAtUtc,
            result.Status,
            result.LatencyMs,
            result.HttpStatusCode,
            result.ErrorMessage)).ToList();
    }

    private async Task<HealthCheckResult> CheckServiceAsync(ServiceEndpoint service, CancellationToken cancellationToken)
    {
        var checkedAt = _clock.UtcNow;

        if (IsSimulatedEndpoint(service.BaseUrl))
        {
            return new HealthCheckResult
            {
                ServiceEndpointId = service.Id,
                CheckedAtUtc = checkedAt,
                Status = HealthStatus.Down,
                LatencyMs = (int)stopwatch.ElapsedMilliseconds,
                HttpStatusCode = null,
                ErrorMessage = $"Request failed: {ex.Message}"
            };
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, service.BaseUrl);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(2);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            stopwatch.Stop();

            var latency = (int)stopwatch.ElapsedMilliseconds;
            var statusCode = (int)response.StatusCode;
            var status = HealthStatusEvaluator.Evaluate(
                response.IsSuccessStatusCode,
                statusCode,
                latency,
                hasException: false);

            return new HealthCheckResult
            {
                ServiceEndpointId = service.Id,
                CheckedAtUtc = checkedAt,
                Status = status,
                LatencyMs = latency,
                HttpStatusCode = statusCode,
                ErrorMessage = status == HealthStatus.Down ? $"HTTP {statusCode}" : null
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new HealthCheckResult
            {
                ServiceEndpointId = service.Id,
                CheckedAtUtc = checkedAt,
                Status = HealthStatus.Down,
                LatencyMs = (int)stopwatch.ElapsedMilliseconds,
                HttpStatusCode = null,
                ErrorMessage = $"Request failed: {ex.Message}"
            };
        }
    }

    private HealthCheckResult CreateSimulatedResult(
        int serviceId,
        DateTime checkedAtUtc,
        string? errorMessage = null)
    {
        var latency = _random.Next(120, 1200);
        var status = latency switch
        {
            >= 900 => HealthStatus.Down,
            >= 500 => HealthStatus.Degraded,
            _ => HealthStatus.Ok
        };

        var httpStatus = status switch
        {
            HealthStatus.Ok => 200,
            HealthStatus.Degraded => 200,
            _ => 503
        };

        return new HealthCheckResult
        {
            ServiceEndpointId = serviceId,
            CheckedAtUtc = checkedAtUtc,
            Status = status,
            LatencyMs = latency,
            HttpStatusCode = httpStatus,
            ErrorMessage = errorMessage ?? (status == HealthStatus.Down ? "Simulated outage." : null)
        };
    }

    private static bool IsSimulatedEndpoint(string baseUrl)
    {
        var lowered = baseUrl.ToLowerInvariant();
        return lowered.Contains("example")
               || lowered.Contains("mock")
               || lowered.Contains("localhost")
               || lowered.Contains("local");
    }
}
