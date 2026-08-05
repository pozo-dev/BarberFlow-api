using BarberFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations
{
    [DbContext(typeof(BarberFlowDbContext))]
    [Migration("20260804211000_EnforceBranchWeeklySchedules")]
    public partial class EnforceBranchWeeklySchedules : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ;WITH DuplicateSchedules AS
                (
                    SELECT Id,
                        ROW_NUMBER() OVER (PARTITION BY BranchId, DayOfWeek ORDER BY Id) AS RowNumber
                    FROM BranchSchedules
                )
                DELETE FROM DuplicateSchedules WHERE RowNumber > 1;

                ;WITH Days AS
                (
                    SELECT CAST(0 AS int) AS DayOfWeek UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL
                    SELECT 3 UNION ALL SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6
                )
                INSERT INTO BranchSchedules (Id, BranchId, DayOfWeek, OpenTime, CloseTime, IsClosed)
                SELECT NEWID(), branch.Id, dayOfWeek.DayOfWeek,
                    CAST('08:00:00' AS time), CAST('18:00:00' AS time), CAST(0 AS bit)
                FROM Branches AS branch
                CROSS JOIN Days AS dayOfWeek
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM BranchSchedules AS schedule
                    WHERE schedule.BranchId = branch.Id
                      AND schedule.DayOfWeek = dayOfWeek.DayOfWeek
                );
                """);

            migrationBuilder.CreateIndex(
                name: "IX_BranchSchedules_BranchId_DayOfWeek",
                table: "BranchSchedules",
                columns: new[] { "BranchId", "DayOfWeek" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BranchSchedules_BranchId_DayOfWeek",
                table: "BranchSchedules");
        }
    }
}
