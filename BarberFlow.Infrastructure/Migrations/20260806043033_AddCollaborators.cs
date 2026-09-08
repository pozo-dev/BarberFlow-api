using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations;
public partial class AddCollaborators : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Collaborators",
            columns: table => new
            {
                Id = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),

                BranchId = table.Column<Guid>(
                    type: "uniqueidentifier",
                    nullable: false),

                FullName = table.Column<string>(
                    type: "nvarchar(120)",
                    maxLength: 120,
                    nullable: false),

                PhoneNumber = table.Column<string>(
                    type: "nvarchar(30)",
                    maxLength: 30,
                    nullable: false),

                RoleId = table.Column<int>(
                    type: "int",
                    nullable: false),

                IsActive = table.Column<bool>(
                    type: "bit",
                    nullable: false,
                    defaultValue: true),

                CreatedAt = table.Column<DateTime>(
                    type: "datetime2",
                    nullable: false),

                UpdatedAt = table.Column<DateTime>(
                    type: "datetime2",
                    nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Collaborators", x => x.Id);

                table.ForeignKey(
                    name: "FK_Collaborators_Branches_BranchId",
                    column: x => x.BranchId,
                    principalTable: "Branches",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);

                table.ForeignKey(
                    name: "FK_Collaborators_Roles_RoleId",
                    column: x => x.RoleId,
                    principalTable: "Roles",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Collaborators_BranchId_IsActive",
            table: "Collaborators",
            columns: new[] { "BranchId", "IsActive" });

        migrationBuilder.CreateIndex(
            name: "IX_Collaborators_RoleId",
            table: "Collaborators",
            column: "RoleId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Collaborators");
    }
}
