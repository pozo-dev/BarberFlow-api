using BarberFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations;
[DbContext(typeof(BarberFlowDbContext))]
[Migration("20260816000000_AddAppointmentBranchAndProfessional")]
public partial class AddAppointmentBranchAndProfessional : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Appointments_BarberShops_BarberShopId", table: "Appointments");
        migrationBuilder.DropIndex(name: "IX_Appointments_BarberShopId", table: "Appointments");
        migrationBuilder.DropColumn(name: "BarberShopId", table: "Appointments");

        migrationBuilder.AddColumn<Guid>(name: "BranchId", table: "Appointments", type: "uniqueidentifier", nullable: false);
        migrationBuilder.AddColumn<Guid>(name: "CollaboratorId", table: "Appointments", type: "uniqueidentifier", nullable: false);

        migrationBuilder.CreateIndex(name: "IX_Appointments_BranchId_CollaboratorId_StartDateTime", table: "Appointments", columns: new[] { "BranchId", "CollaboratorId", "StartDateTime" });
        migrationBuilder.CreateIndex(name: "IX_Appointments_CollaboratorId", table: "Appointments", column: "CollaboratorId");
        migrationBuilder.CreateIndex(name: "IX_Appointments_BranchId", table: "Appointments", column: "BranchId");
        migrationBuilder.AddForeignKey(name: "FK_Appointments_Collaborators_CollaboratorId", table: "Appointments", column: "CollaboratorId", principalTable: "Collaborators", principalColumn: "Id", onDelete: ReferentialAction.NoAction);
        migrationBuilder.AddForeignKey(name: "FK_Appointments_Branches_BranchId", table: "Appointments", column: "BranchId", principalTable: "Branches", principalColumn: "Id", onDelete: ReferentialAction.NoAction);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(name: "BarberShopId", table: "Appointments", type: "uniqueidentifier", nullable: true);
        migrationBuilder.DropForeignKey(name: "FK_Appointments_Collaborators_CollaboratorId", table: "Appointments");
        migrationBuilder.DropForeignKey(name: "FK_Appointments_Branches_BranchId", table: "Appointments");
        migrationBuilder.DropIndex(name: "IX_Appointments_BranchId_CollaboratorId_StartDateTime", table: "Appointments");
        migrationBuilder.DropIndex(name: "IX_Appointments_CollaboratorId", table: "Appointments");
        migrationBuilder.DropIndex(name: "IX_Appointments_BranchId", table: "Appointments");
        migrationBuilder.DropColumn(name: "CollaboratorId", table: "Appointments");
        migrationBuilder.DropColumn(name: "BranchId", table: "Appointments");
        migrationBuilder.CreateIndex(name: "IX_Appointments_BarberShopId", table: "Appointments", column: "BarberShopId");
        migrationBuilder.AddForeignKey(name: "FK_Appointments_BarberShops_BarberShopId", table: "Appointments", column: "BarberShopId", principalTable: "BarberShops", principalColumn: "Id", onDelete: ReferentialAction.NoAction);
    }
}
