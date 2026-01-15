using Dashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dashboard.Infrastructure.Migrations;

[DbContext(typeof(DashboardDbContext))]
public partial class DashboardDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "8.0.11")
            .HasAnnotation("Relational:MaxIdentifierLength", 63);

        NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

        modelBuilder.Entity("Dashboard.Domain.Entities.HealthCheckResult", b =>
        {
            b.Property<long>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            b.Property<DateTime>("CheckedAtUtc")
                .HasColumnType("timestamp with time zone");

            b.Property<string>("ErrorMessage")
                .HasMaxLength(1000)
                .HasColumnType("character varying(1000)");

            b.Property<int?>("HttpStatusCode")
                .HasColumnType("integer");

            b.Property<int?>("LatencyMs")
                .HasColumnType("integer");

            b.Property<int>("ServiceEndpointId")
                .HasColumnType("integer");

            b.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)");

            b.HasKey("Id");

            b.HasIndex("ServiceEndpointId");

            b.ToTable("HealthCheckResults");
        });

        modelBuilder.Entity("Dashboard.Domain.Entities.Incident", b =>
        {
            b.Property<long>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            b.Property<string>("Description")
                .HasMaxLength(2000)
                .HasColumnType("character varying(2000)");

            b.Property<DateTime>("ReportedAtUtc")
                .HasColumnType("timestamp with time zone");

            b.Property<string>("ReportedBy")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("character varying(120)");

            b.Property<DateTime?>("ResolvedAtUtc")
                .HasColumnType("timestamp with time zone");

            b.Property<string>("ResolutionNotes")
                .HasMaxLength(2000)
                .HasColumnType("character varying(2000)");

            b.Property<int>("ServiceEndpointId")
                .HasColumnType("integer");

            b.Property<string>("Severity")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)");

            b.Property<string>("Status")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("character varying(20)");

            b.Property<string>("Title")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("character varying(200)");

            b.HasKey("Id");

            b.HasIndex("ServiceEndpointId");

            b.ToTable("Incidents");
        });

        modelBuilder.Entity("Dashboard.Domain.Entities.ServiceEndpoint", b =>
        {
            b.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("integer")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            b.Property<string>("BaseUrl")
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            b.Property<DateTime>("CreatedAtUtc")
                .HasColumnType("timestamp with time zone");

            b.Property<string>("Description")
                .HasMaxLength(500)
                .HasColumnType("character varying(500)");

            b.Property<bool>("IsActive")
                .HasColumnType("boolean");

            b.Property<string>("Name")
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("character varying(200)");

            b.Property<string>("OwnerTeam")
                .HasMaxLength(100)
                .HasColumnType("character varying(100)");

            b.HasKey("Id");

            b.ToTable("ServiceEndpoints");
        });

        modelBuilder.Entity("Dashboard.Domain.Entities.HealthCheckResult", b =>
        {
            b.HasOne("Dashboard.Domain.Entities.ServiceEndpoint", "ServiceEndpoint")
                .WithMany("HealthChecks")
                .HasForeignKey("ServiceEndpointId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("ServiceEndpoint");
        });

        modelBuilder.Entity("Dashboard.Domain.Entities.Incident", b =>
        {
            b.HasOne("Dashboard.Domain.Entities.ServiceEndpoint", "ServiceEndpoint")
                .WithMany("Incidents")
                .HasForeignKey("ServiceEndpointId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.Navigation("ServiceEndpoint");
        });

        modelBuilder.Entity("Dashboard.Domain.Entities.ServiceEndpoint", b =>
        {
            b.Navigation("HealthChecks");

            b.Navigation("Incidents");
        });
    }
}
