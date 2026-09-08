using BarberFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations;
/// <summary>
/// Stores all audit and appointment instants with an explicit UTC offset.
/// Business schedules remain SQL time values by design.
/// </summary>
[DbContext(typeof(BarberFlowDbContext))]
[Migration("20260826000000_StandardizeUtcDateTimes")]
public partial class StandardizeUtcDateTimes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // The conditional form also makes a failed development startup
        // recoverable if SQL Server already applied this first statement.
        migrationBuilder.Sql("""
            IF COL_LENGTH('Branches', 'TimeZoneId') IS NULL
                ALTER TABLE [Branches] ADD [TimeZoneId] nvarchar(100) NOT NULL DEFAULT N'America/Managua';
            """);

        migrationBuilder.DropIndex(name: "IX_Appointments_BranchId_CollaboratorId_StartDateTime", table: "Appointments");
        migrationBuilder.DropIndex(name: "IX_OtpCodes_UserId_CreatedAt", table: "OtpCodes");
        migrationBuilder.DropIndex(name: "IX_OtpCodes_UserId_IsUsed_ExpiresAt", table: "OtpCodes");

        AlterToUtc(migrationBuilder, "Appointments", "CreatedAt");
        AlterToUtc(migrationBuilder, "Appointments", "EndDateTime");
        AlterToUtc(migrationBuilder, "Appointments", "StartDateTime");
        AlterToUtc(migrationBuilder, "BarberAssignments", "CreatedAt");
        AlterToUtc(migrationBuilder, "BarberAssignments", "UpdatedAt", nullable: true);
        AlterToUtc(migrationBuilder, "BarberShops", "CreatedAt");
        AlterToUtc(migrationBuilder, "Branches", "CreatedAt");
        AlterToUtc(migrationBuilder, "Branches", "UpdatedAt", nullable: true);
        AlterToUtc(migrationBuilder, "Collaborators", "CreatedAt");
        AlterToUtc(migrationBuilder, "Collaborators", "UpdatedAt", nullable: true);
        AlterToUtc(migrationBuilder, "OtpCodes", "CreatedAt");
        AlterToUtc(migrationBuilder, "OtpCodes", "ExpiresAt");
        AlterToUtc(migrationBuilder, "RefreshTokens", "CreatedAt");
        AlterToUtc(migrationBuilder, "RefreshTokens", "ExpiresAt");
        AlterToUtc(migrationBuilder, "RefreshTokens", "RevokedAt", nullable: true);
        AlterToUtc(migrationBuilder, "Services", "CreatedAt");
        AlterToUtc(migrationBuilder, "Services", "UpdatedAt", nullable: true);
        AlterToUtc(migrationBuilder, "ServicePrices", "CreatedAt");
        AlterToUtc(migrationBuilder, "ServicePrices", "EffectiveFrom");
        AlterToUtc(migrationBuilder, "Users", "CreatedAt");
        AlterToUtc(migrationBuilder, "UserProfiles", "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Appointments_BranchId_CollaboratorId_StartDateTime",
            table: "Appointments",
            columns: new[] { "BranchId", "CollaboratorId", "StartDateTime" });
        migrationBuilder.CreateIndex(
            name: "IX_OtpCodes_UserId_CreatedAt",
            table: "OtpCodes",
            columns: new[] { "UserId", "CreatedAt" });
        migrationBuilder.CreateIndex(
            name: "IX_OtpCodes_UserId_IsUsed_ExpiresAt",
            table: "OtpCodes",
            columns: new[] { "UserId", "IsUsed", "ExpiresAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_Appointments_BranchId_CollaboratorId_StartDateTime", table: "Appointments");
        migrationBuilder.DropIndex(name: "IX_OtpCodes_UserId_CreatedAt", table: "OtpCodes");
        migrationBuilder.DropIndex(name: "IX_OtpCodes_UserId_IsUsed_ExpiresAt", table: "OtpCodes");
        AlterToDateTime(migrationBuilder, "Appointments", "CreatedAt");
        AlterToDateTime(migrationBuilder, "Appointments", "EndDateTime");
        AlterToDateTime(migrationBuilder, "Appointments", "StartDateTime");
        AlterToDateTime(migrationBuilder, "BarberAssignments", "CreatedAt");
        AlterToDateTime(migrationBuilder, "BarberAssignments", "UpdatedAt", nullable: true);
        AlterToDateTime(migrationBuilder, "BarberShops", "CreatedAt");
        AlterToDateTime(migrationBuilder, "Branches", "CreatedAt");
        AlterToDateTime(migrationBuilder, "Branches", "UpdatedAt", nullable: true);
        AlterToDateTime(migrationBuilder, "Collaborators", "CreatedAt");
        AlterToDateTime(migrationBuilder, "Collaborators", "UpdatedAt", nullable: true);
        AlterToDateTime(migrationBuilder, "OtpCodes", "CreatedAt");
        AlterToDateTime(migrationBuilder, "OtpCodes", "ExpiresAt");
        AlterToDateTime(migrationBuilder, "RefreshTokens", "CreatedAt");
        AlterToDateTime(migrationBuilder, "RefreshTokens", "ExpiresAt");
        AlterToDateTime(migrationBuilder, "RefreshTokens", "RevokedAt", nullable: true);
        AlterToDateTime(migrationBuilder, "Services", "CreatedAt");
        AlterToDateTime(migrationBuilder, "Services", "UpdatedAt", nullable: true);
        AlterToDateTime(migrationBuilder, "ServicePrices", "CreatedAt");
        AlterToDateTime(migrationBuilder, "ServicePrices", "EffectiveFrom");
        AlterToDateTime(migrationBuilder, "Users", "CreatedAt");
        AlterToDateTime(migrationBuilder, "UserProfiles", "CreatedAt");

        migrationBuilder.CreateIndex(
            name: "IX_Appointments_BranchId_CollaboratorId_StartDateTime",
            table: "Appointments",
            columns: new[] { "BranchId", "CollaboratorId", "StartDateTime" });
        migrationBuilder.CreateIndex(
            name: "IX_OtpCodes_UserId_CreatedAt",
            table: "OtpCodes",
            columns: new[] { "UserId", "CreatedAt" });
        migrationBuilder.CreateIndex(
            name: "IX_OtpCodes_UserId_IsUsed_ExpiresAt",
            table: "OtpCodes",
            columns: new[] { "UserId", "IsUsed", "ExpiresAt" });

        migrationBuilder.DropColumn(name: "TimeZoneId", table: "Branches");
    }

    private static void AlterToUtc(MigrationBuilder migrationBuilder, string table, string name, bool nullable = false) =>
        migrationBuilder.AlterColumn<DateTimeOffset>(name: name, table: table, type: "datetimeoffset", nullable: nullable, oldClrType: typeof(DateTime), oldType: "datetime2", oldNullable: nullable);

    private static void AlterToDateTime(MigrationBuilder migrationBuilder, string table, string name, bool nullable = false) =>
        migrationBuilder.AlterColumn<DateTime>(name: name, table: table, type: "datetime2", nullable: nullable, oldClrType: typeof(DateTimeOffset), oldType: "datetimeoffset", oldNullable: nullable);
}
