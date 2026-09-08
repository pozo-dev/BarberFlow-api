using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvailabilityApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CollaboratorTimeOff_Type",
                table: "CollaboratorTimeOff");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByProfileId",
                table: "CollaboratorTimeOff",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReviewedAtUtc",
                table: "CollaboratorTimeOff",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedByProfileId",
                table: "CollaboratorTimeOff",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CollaboratorTimeOff",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateTable(
                name: "CollaboratorScheduleRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CollaboratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UseBranchHours = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedByProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ReviewedByProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaboratorScheduleRequests", x => x.Id);
                    table.CheckConstraint("CK_ScheduleRequest_Status", "[Status] BETWEEN 1 AND 4");
                    table.ForeignKey(
                        name: "FK_CollaboratorScheduleRequests_Collaborators_CollaboratorId",
                        column: x => x.CollaboratorId,
                        principalTable: "Collaborators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CollaboratorRequestedPeriods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollaboratorRequestedPeriods", x => x.Id);
                    table.CheckConstraint("CK_RequestedPeriod_Day", "[DayOfWeek] BETWEEN 1 AND 7");
                    table.CheckConstraint("CK_RequestedPeriod_Time", "[StartTime] < [EndTime]");
                    table.ForeignKey(
                        name: "FK_CollaboratorRequestedPeriods_CollaboratorScheduleRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "CollaboratorScheduleRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollaboratorTimeOff_Status",
                table: "CollaboratorTimeOff",
                sql: "[Status] BETWEEN 1 AND 4");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollaboratorTimeOff_Type",
                table: "CollaboratorTimeOff",
                sql: "[Type] IN (1, 2, 3)");

            migrationBuilder.CreateIndex(
                name: "IX_CollaboratorRequestedPeriods_RequestId",
                table: "CollaboratorRequestedPeriods",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CollaboratorScheduleRequests_CollaboratorId",
                table: "CollaboratorScheduleRequests",
                column: "CollaboratorId",
                unique: true,
                filter: "[Status] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollaboratorRequestedPeriods");

            migrationBuilder.DropTable(
                name: "CollaboratorScheduleRequests");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollaboratorTimeOff_Status",
                table: "CollaboratorTimeOff");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CollaboratorTimeOff_Type",
                table: "CollaboratorTimeOff");

            migrationBuilder.DropColumn(
                name: "CreatedByProfileId",
                table: "CollaboratorTimeOff");

            migrationBuilder.DropColumn(
                name: "ReviewedAtUtc",
                table: "CollaboratorTimeOff");

            migrationBuilder.DropColumn(
                name: "ReviewedByProfileId",
                table: "CollaboratorTimeOff");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CollaboratorTimeOff");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CollaboratorTimeOff_Type",
                table: "CollaboratorTimeOff",
                sql: "[Type] IN (1, 2)");
        }
    }
}
