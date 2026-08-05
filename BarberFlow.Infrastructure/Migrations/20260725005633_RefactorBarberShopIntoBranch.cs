using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorBarberShopIntoBranch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
