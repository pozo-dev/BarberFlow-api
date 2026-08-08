using BarberFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations
{
    [DbContext(typeof(BarberFlowDbContext))]
    [Migration("20260807120000_AddBarberShopImages")]
    public partial class AddBarberShopImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Banner",
                table: "BarberShops",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: Array.Empty<byte>());

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "BarberShops",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: Array.Empty<byte>());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Banner", table: "BarberShops");
            migrationBuilder.DropColumn(name: "Logo", table: "BarberShops");
        }
    }
}
