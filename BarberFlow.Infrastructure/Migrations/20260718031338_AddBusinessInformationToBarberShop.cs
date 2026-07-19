using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessInformationToBarberShop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "BarberShops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "BarberShops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "CloseTime",
                table: "BarberShops",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "OpenTime",
                table: "BarberShops",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "BarberShops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "BarberShops");

            migrationBuilder.DropColumn(
                name: "City",
                table: "BarberShops");

            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "BarberShops");

            migrationBuilder.DropColumn(
                name: "OpenTime",
                table: "BarberShops");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "BarberShops");
        }
    }
}
