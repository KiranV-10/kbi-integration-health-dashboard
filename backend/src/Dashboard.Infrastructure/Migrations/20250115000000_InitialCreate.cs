using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dashboard.Infrastructure.Migrations;

[DbContext(typeof(Data.DashboardDbContext))]
[Migration("20250115000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ServiceEndpoints",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                BaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                OwnerTeam = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ServiceEndpoints", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "HealthCheckResults",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ServiceEndpointId = table.Column<int>(type: "integer", nullable: false),
                CheckedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                LatencyMs = table.Column<int>(type: "integer", nullable: true),
                HttpStatusCode = table.Column<int>(type: "integer", nullable: true),
                ErrorMessage = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HealthCheckResults", x => x.Id);
                table.ForeignKey(
                    name: "FK_HealthCheckResults_ServiceEndpoints_ServiceEndpointId",
                    column: x => x.ServiceEndpointId,
                    principalTable: "ServiceEndpoints",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Incidents",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ServiceEndpointId = table.Column<int>(type: "integer", nullable: false),
                Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Severity = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                ReportedBy = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                ReportedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                ResolutionNotes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                ResolvedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Incidents", x => x.Id);
                table.ForeignKey(
                    name: "FK_Incidents_ServiceEndpoints_ServiceEndpointId",
                    column: x => x.ServiceEndpointId,
                    principalTable: "ServiceEndpoints",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_HealthCheckResults_ServiceEndpointId",
            table: "HealthCheckResults",
            column: "ServiceEndpointId");

        migrationBuilder.CreateIndex(
            name: "IX_Incidents_ServiceEndpointId",
            table: "Incidents",
            column: "ServiceEndpointId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "HealthCheckResults");

        migrationBuilder.DropTable(
            name: "Incidents");

        migrationBuilder.DropTable(
            name: "ServiceEndpoints");
    }
}
