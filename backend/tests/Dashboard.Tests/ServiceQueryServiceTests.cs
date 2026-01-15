using Dashboard.Application.Services;
using Dashboard.Domain.Entities;
using Dashboard.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dashboard.Tests;

public class ServiceQueryServiceTests
{
    [Fact]
    public async Task GetServices_ReturnsLatestHealthCheckAndOpenIncidentCount()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new TestDbContext(options);
        var service = new ServiceEndpoint
        {
            Name = "Records API",
            BaseUrl = "https://example.com",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.ServiceEndpoints.Add(service);
        await db.SaveChangesAsync();

        db.HealthCheckResults.AddRange(
            new HealthCheckResult
            {
                ServiceEndpointId = service.Id,
                CheckedAtUtc = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc),
                Status = HealthStatus.Ok,
                LatencyMs = 120
            },
            new HealthCheckResult
            {
                ServiceEndpointId = service.Id,
                CheckedAtUtc = new DateTime(2024, 1, 1, 11, 0, 0, DateTimeKind.Utc),
                Status = HealthStatus.Degraded,
                LatencyMs = 620,
                ErrorMessage = "Slow response"
            });

        db.Incidents.AddRange(
            new Incident
            {
                ServiceEndpointId = service.Id,
                Title = "Latency spike",
                Severity = IncidentSeverity.Medium,
                Status = IncidentStatus.Open,
                ReportedBy = "Ops",
                ReportedAtUtc = DateTime.UtcNow
            },
            new Incident
            {
                ServiceEndpointId = service.Id,
                Title = "Resolved issue",
                Severity = IncidentSeverity.Low,
                Status = IncidentStatus.Resolved,
                ReportedBy = "Ops",
                ReportedAtUtc = DateTime.UtcNow
            });

        await db.SaveChangesAsync();

        var serviceUnderTest = new ServiceQueryService(db);
        var results = await serviceUnderTest.GetServicesAsync(true, null, CancellationToken.None);
        var result = results.Single();

        Assert.Equal(HealthStatus.Degraded, result.LastStatus);
        Assert.Equal(620, result.LatencyMs);
        Assert.Equal("Slow response", result.LastErrorMessage);
        Assert.Equal(1, result.OpenIncidentCount);
    }

    [Fact]
    public async Task GetServices_RespectsActiveOnlyFilter()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var db = new TestDbContext(options);
        db.ServiceEndpoints.AddRange(
            new ServiceEndpoint
            {
                Name = "Active API",
                BaseUrl = "https://example.com",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            },
            new ServiceEndpoint
            {
                Name = "Inactive API",
                BaseUrl = "https://example.com",
                IsActive = false,
                CreatedAtUtc = DateTime.UtcNow
            });
        await db.SaveChangesAsync();

        var serviceUnderTest = new ServiceQueryService(db);
        var results = await serviceUnderTest.GetServicesAsync(true, null, CancellationToken.None);

        Assert.Single(results);
        Assert.Equal("Active API", results[0].Name);
    }
}
