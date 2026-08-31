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
            // Keeps a database that stopped on the previous version of this migration
            // consistent with the Branch schema before schedules are initialized.
            migrationBuilder.Sql("""
                IF COL_LENGTH(N'dbo.Branches', N'City') IS NULL
                BEGIN
                    ALTER TABLE dbo.Branches
                    ADD City nvarchar(120) NOT NULL CONSTRAINT DF_Branches_City DEFAULT N'';
                END;

                IF COL_LENGTH(N'dbo.Branches', N'IsMain') IS NULL
                BEGIN
                    ALTER TABLE dbo.Branches
                    ADD IsMain bit NOT NULL CONSTRAINT DF_Branches_IsMain DEFAULT CAST(0 AS bit);
                END;
                """);

            migrationBuilder.CreateTable(
                name: "BranchSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    OpenTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    CloseTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchSchedules_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    SELECT CAST(1 AS int) AS DayOfWeek UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL
                    SELECT 4 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7
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
            migrationBuilder.DropTable(
                name: "BranchSchedules");
        }
    }
}
