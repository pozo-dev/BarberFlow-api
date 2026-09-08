using BarberFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations;
[DbContext(typeof(BarberFlowDbContext))]
[Migration("20260827000000_AddAppointmentRescheduleTracking")]
public partial class AddAppointmentRescheduleTracking : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "RescheduledToAppointmentId",
            table: "Appointments",
            type: "uniqueidentifier",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RescheduledToAppointmentId",
            table: "Appointments");
    }
}
