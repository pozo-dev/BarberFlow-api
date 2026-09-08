using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations;
/// <inheritdoc />
public partial class AddAppointmentActivityHistory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AppointmentActivities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                ActorProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ActorRoleId = table.Column<int>(type: "int", nullable: false),
                OccurredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                PreviousStartAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                NewStartAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                RelatedAppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AppointmentActivities", x => x.Id);
                table.ForeignKey(
                    name: "FK_AppointmentActivities_Appointments_AppointmentId",
                    column: x => x.AppointmentId,
                    principalTable: "Appointments",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_AppointmentActivities_AppointmentId_OccurredAtUtc",
            table: "AppointmentActivities",
            columns: new[] { "AppointmentId", "OccurredAtUtc" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AppointmentActivities");
    }
}
