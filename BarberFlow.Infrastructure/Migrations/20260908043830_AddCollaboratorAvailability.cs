using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations;
/// <inheritdoc />
public partial class AddCollaboratorAvailability : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CollaboratorTimeOff",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CollaboratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                AllDay = table.Column<bool>(type: "bit", nullable: false),
                StartAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                EndAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CollaboratorTimeOff", x => x.Id);
                table.CheckConstraint("CK_CollaboratorTimeOff_Range", "[StartAtUtc] < [EndAtUtc]");
                table.CheckConstraint("CK_CollaboratorTimeOff_Type", "[Type] IN (1, 2)");
                table.ForeignKey(
                    name: "FK_CollaboratorTimeOff_Collaborators_CollaboratorId",
                    column: x => x.CollaboratorId,
                    principalTable: "Collaborators",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "CollaboratorWorkingHours",
            columns: table => new
            {
                CollaboratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UseBranchHours = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CollaboratorWorkingHours", x => x.CollaboratorId);
                table.ForeignKey(
                    name: "FK_CollaboratorWorkingHours_Collaborators_CollaboratorId",
                    column: x => x.CollaboratorId,
                    principalTable: "Collaborators",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "CollaboratorWorkPeriods",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CollaboratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DayOfWeek = table.Column<int>(type: "int", nullable: false),
                StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CollaboratorWorkPeriods", x => x.Id);
                table.CheckConstraint("CK_CollaboratorWorkPeriods_Day", "[DayOfWeek] BETWEEN 1 AND 7");
                table.CheckConstraint("CK_CollaboratorWorkPeriods_Time", "[StartTime] < [EndTime]");
                table.ForeignKey(
                    name: "FK_CollaboratorWorkPeriods_CollaboratorWorkingHours_CollaboratorId",
                    column: x => x.CollaboratorId,
                    principalTable: "CollaboratorWorkingHours",
                    principalColumn: "CollaboratorId",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CollaboratorTimeOff_CollaboratorId_StartAtUtc_EndAtUtc",
            table: "CollaboratorTimeOff",
            columns: new[] { "CollaboratorId", "StartAtUtc", "EndAtUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_CollaboratorWorkPeriods_CollaboratorId_DayOfWeek",
            table: "CollaboratorWorkPeriods",
            columns: new[] { "CollaboratorId", "DayOfWeek" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CollaboratorTimeOff");

        migrationBuilder.DropTable(
            name: "CollaboratorWorkPeriods");

        migrationBuilder.DropTable(
            name: "CollaboratorWorkingHours");
    }
}
