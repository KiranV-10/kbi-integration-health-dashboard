using Dashboard.Application.Models;
using Dashboard.Application.Services;
using Dashboard.Domain.Entities;
using Dashboard.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dashboard.Tests;

public class IncidentServiceTests
{
    [Fact]
    public async Task CreateIncident_Throws_WhenTitleMissing()
    {
        var service = BuildService();
        var db = BuildDbContext(service);
        var clock = new TestDateTimeProvider(DateTime.UtcNow);
        var serviceUnderTest = new IncidentService(db, clock);

        var request = new IncidentCreateRequest
        {
            ServiceEndpointId = service.Id,
            Title = " ",
            Severity = IncidentSeverity.High,
            ReportedBy = "Analyst",
            Description = "Test"
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            serviceUnderTest.CreateIncidentAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateIncident_Throws_WhenReportedByMissing()
    {
        var service = BuildService();
        var db = BuildDbContext(service);
        var clock = new TestDateTimeProvider(DateTime.UtcNow);
        var serviceUnderTest = new IncidentService(db, clock);

        var request = new IncidentCreateRequest
        {
            ServiceEndpointId = service.Id,
            Title = "Test",
            Severity = IncidentSeverity.High,
            ReportedBy = " ",
            Description = "Test"
        };

        await Assert.ThrowsAsync<ArgumentException>(() =>
            serviceUnderTest.CreateIncidentAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateIncident_SetsStatusAndReportedAt()
    {
        var service = BuildService();
        var db = BuildDbContext(service);
        var now = new DateTime(2024, 12, 1, 10, 0, 0, DateTimeKind.Utc);
        var clock = new TestDateTimeProvider(now);
        var serviceUnderTest = new IncidentService(db, clock);

        var request = new IncidentCreateRequest
        {
            ServiceEndpointId = service.Id,
            Title = "API outage",
            Severity = IncidentSeverity.Critical,
            ReportedBy = "On-call",
            Description = "Synthetic test"
        };

        var result = await serviceUnderTest.CreateIncidentAsync(request, CancellationToken.None);

        Assert.Equal(IncidentStatus.Open, result.Status);
        Assert.Equal(now, result.ReportedAtUtc);
    }

    private static TestDbContext BuildDbContext(ServiceEndpoint service)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new TestDbContext(options);
        db.ServiceEndpoints.Add(service);
        db.SaveChanges();
        return db;
    }

    private static ServiceEndpoint BuildService()
    {
        return new ServiceEndpoint
        {
            Name = "Test Service",
            BaseUrl = "https://example.com",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}
