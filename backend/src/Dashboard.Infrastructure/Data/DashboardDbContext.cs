using Dashboard.Application.Abstractions;
using Dashboard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dashboard.Infrastructure.Data;

public class DashboardDbContext : DbContext, IAppDbContext
{
    public DashboardDbContext(DbContextOptions<DashboardDbContext> options)
        : base(options)
    {
    }

    public DbSet<ServiceEndpoint> ServiceEndpoints => Set<ServiceEndpoint>();
    public DbSet<HealthCheckResult> HealthCheckResults => Set<HealthCheckResult>();
    public DbSet<Incident> Incidents => Set<Incident>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceEndpoint>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.BaseUrl).HasMaxLength(500).IsRequired();
            entity.Property(x => x.OwnerTeam).HasMaxLength(100);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<HealthCheckResult>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(x => x.ErrorMessage).HasMaxLength(1000);
            entity.HasOne(x => x.ServiceEndpoint)
                .WithMany(s => s.HealthChecks)
                .HasForeignKey(x => x.ServiceEndpointId);
        });

        modelBuilder.Entity<Incident>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ReportedBy).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.ResolutionNotes).HasMaxLength(2000);
            entity.Property(x => x.Severity).HasConversion<string>().HasMaxLength(20);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasOne(x => x.ServiceEndpoint)
                .WithMany(s => s.Incidents)
                .HasForeignKey(x => x.ServiceEndpointId);
        });
    }
}
