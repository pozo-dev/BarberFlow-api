using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations;
/// <inheritdoc />
public partial class CompleteServiceEntity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Services_BarberShopId",
            table: "Services");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Services",
            type: "nvarchar(80)",
            maxLength: 80,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(100)",
            oldMaxLength: 100);

        migrationBuilder.AlterColumn<bool>(
            name: "IsActive",
            table: "Services",
            type: "bit",
            nullable: false,
            defaultValue: true,
            oldClrType: typeof(bool),
            oldType: "bit");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Services",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(300)",
            oldMaxLength: 300);

        migrationBuilder.AddColumn<int>(
            name: "DisplayOrder",
            table: "Services",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<TimeSpan>(
            name: "Duration",
            table: "Services",
            type: "time",
            nullable: false,
            defaultValue: new TimeSpan(0, 0, 0, 0, 0));

        migrationBuilder.AddColumn<decimal>(
            name: "Price",
            table: "Services",
            type: "decimal(10,2)",
            precision: 10,
            scale: 2,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<DateTime>(
            name: "UpdatedAt",
            table: "Services",
            type: "datetime2",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Services_BarberShopId_IsActive",
            table: "Services",
            columns: new[] { "BarberShopId", "IsActive" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Services_BarberShopId_IsActive",
            table: "Services");

        migrationBuilder.DropColumn(
            name: "DisplayOrder",
            table: "Services");

        migrationBuilder.DropColumn(
            name: "Duration",
            table: "Services");

        migrationBuilder.DropColumn(
            name: "Price",
            table: "Services");

        migrationBuilder.DropColumn(
            name: "UpdatedAt",
            table: "Services");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            table: "Services",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(80)",
            oldMaxLength: 80);

        migrationBuilder.AlterColumn<bool>(
            name: "IsActive",
            table: "Services",
            type: "bit",
            nullable: false,
            oldClrType: typeof(bool),
            oldType: "bit",
            oldDefaultValue: true);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "Services",
            type: "nvarchar(300)",
            maxLength: 300,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Services_BarberShopId",
            table: "Services",
            column: "BarberShopId");
    }
}
