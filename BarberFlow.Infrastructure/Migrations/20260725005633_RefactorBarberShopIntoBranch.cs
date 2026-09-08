using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations;
/// <inheritdoc />
public partial class RefactorBarberShopIntoBranch : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "City",
            table: "Branches",
            type: "nvarchar(120)",
            maxLength: 120,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<bool>(
            name: "IsMain",
            table: "Branches",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.DropForeignKey(
            name: "FK_Appointments_BarberShops_BarberShopId",
            table: "Appointments");

        migrationBuilder.AddForeignKey(
            name: "FK_Appointments_BarberShops_BarberShopId",
            table: "Appointments",
            column: "BarberShopId",
            principalTable: "BarberShops",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Appointments_BarberShops_BarberShopId",
            table: "Appointments");

        migrationBuilder.AddForeignKey(
            name: "FK_Appointments_BarberShops_BarberShopId",
            table: "Appointments",
            column: "BarberShopId",
            principalTable: "BarberShops",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.DropColumn(
            name: "City",
            table: "Branches");

        migrationBuilder.DropColumn(
            name: "IsMain",
            table: "Branches");
    }
}
