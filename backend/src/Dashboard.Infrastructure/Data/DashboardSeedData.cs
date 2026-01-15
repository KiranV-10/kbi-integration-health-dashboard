using Dashboard.Application.Abstractions;
using Dashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Infrastructure.Data;

public static class DashboardSeedData
{
    public static async Task EnsureSeededAsync(DashboardDbContext dbContext, IDateTimeProvider clock)
    {
        if (await dbContext.ServiceEndpoints.AnyAsync())
        {
            return;
        }

        var now = clock.UtcNow;
        var services = new List<ServiceEndpoint>
        {
            new()
            {
                Name = "Court Docket API",
                Description = "Public court docket search integration.",
                BaseUrl = "https://example.com/court-docket",
                OwnerTeam = "Justice IT",
                IsActive = true,
                CreatedAtUtc = now
            },
            new()
            {
                Name = "DMV Lookup Service",
                Description = "Driver record lookup for field officers.",
                BaseUrl = "https://example.com/dmv-lookup",
                OwnerTeam = "Transportation IT",
                IsActive = true,
                CreatedAtUtc = now
            },
            new()
            {
                Name = "Local PD RMS",
                Description = "Records management system integration.",
                BaseUrl = "https://example.com/local-pd-rms",
                OwnerTeam = "Public Safety",
                IsActive = true,
                CreatedAtUtc = now
            },
            new()
            {
                Name = "Statewide Warrants Hub",
                Description = "Aggregated warrants search gateway.",
                BaseUrl = "https://example.com/warrants-hub",
                OwnerTeam = "Criminal Justice",
                IsActive = true,
                CreatedAtUtc = now
            },
            new()
            {
                Name = "Inmate Locator",
                Description = "Corrections inmate locator integration.",
                BaseUrl = "https://example.com/inmate-locator",
                OwnerTeam = "Corrections",
                IsActive = true,
                CreatedAtUtc = now
            },
            new()
            {
                Name = "CJIS Messaging Gateway",
                Description = "Secure CJIS message relay.",
                BaseUrl = "https://example.com/cjis-gateway",
                OwnerTeam = "Security Operations",
                IsActive = true,
                CreatedAtUtc = now
            },
            new()
            {
                Name = "NCIC Query Adapter",
                Description = "Broker for NCIC searches.",
                BaseUrl = "https://example.com/ncic-query",
                OwnerTeam = "Integration Team",
                IsActive = true,
                CreatedAtUtc = now
            },
            new()
            {
                Name = "911 CAD Feed",
                Description = "Computer-aided dispatch feed.",
                BaseUrl = "https://example.com/911-cad",
                OwnerTeam = "Emergency Services",
                IsActive = true,
                CreatedAtUtc = now
            },
            new()
            {
                Name = "Lab Results Exchange",
                Description = "Forensics lab results delivery.",
                BaseUrl = "https://example.com/lab-results",
                OwnerTeam = "Forensics",
                IsActive = true,
                CreatedAtUtc = now
            }
        };

        dbContext.ServiceEndpoints.AddRange(services);
        await dbContext.SaveChangesAsync(CancellationToken.None);
    }
}
