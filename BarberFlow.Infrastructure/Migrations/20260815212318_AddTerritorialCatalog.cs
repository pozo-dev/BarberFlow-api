using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations;
/// <inheritdoc />
public partial class AddTerritorialCatalog : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "LocationSearchId",
            table: "Branches",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.CreateTable(
            name: "Countries",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Countries", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CountryAdministrativeLevels",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CountryId = table.Column<int>(type: "int", nullable: false),
                Level = table.Column<int>(type: "int", nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CountryAdministrativeLevels", x => x.Id);
                table.ForeignKey(
                    name: "FK_CountryAdministrativeLevels_Countries_CountryId",
                    column: x => x.CountryId,
                    principalTable: "Countries",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AdministrativeAreaTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CountryAdministrativeLevelId = table.Column<int>(type: "int", nullable: false),
                Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AdministrativeAreaTypes", x => x.Id);
                table.ForeignKey(
                    name: "FK_AdministrativeAreaTypes_CountryAdministrativeLevels_CountryAdministrativeLevelId",
                    column: x => x.CountryAdministrativeLevelId,
                    principalTable: "CountryAdministrativeLevels",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AdministrativeAreas",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CountryAdministrativeLevelId = table.Column<int>(type: "int", nullable: false),
                AdministrativeAreaTypeId = table.Column<int>(type: "int", nullable: false),
                ParentId = table.Column<int>(type: "int", nullable: true),
                Code = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AdministrativeAreas", x => x.Id);
                table.ForeignKey(
                    name: "FK_AdministrativeAreas_AdministrativeAreaTypes_AdministrativeAreaTypeId",
                    column: x => x.AdministrativeAreaTypeId,
                    principalTable: "AdministrativeAreaTypes",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AdministrativeAreas_AdministrativeAreas_ParentId",
                    column: x => x.ParentId,
                    principalTable: "AdministrativeAreas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_AdministrativeAreas_CountryAdministrativeLevels_CountryAdministrativeLevelId",
                    column: x => x.CountryAdministrativeLevelId,
                    principalTable: "CountryAdministrativeLevels",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "LocationSearches",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                AdministrativeAreaId = table.Column<int>(type: "int", nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                SearchText = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LocationSearches", x => x.Id);
                table.ForeignKey(
                    name: "FK_LocationSearches_AdministrativeAreas_AdministrativeAreaId",
                    column: x => x.AdministrativeAreaId,
                    principalTable: "AdministrativeAreas",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Branches_LocationSearchId",
            table: "Branches",
            column: "LocationSearchId");

        migrationBuilder.CreateIndex(
            name: "IX_AdministrativeAreas_AdministrativeAreaTypeId",
            table: "AdministrativeAreas",
            column: "AdministrativeAreaTypeId");

        migrationBuilder.CreateIndex(
            name: "IX_AdministrativeAreas_CountryAdministrativeLevelId_ParentId_Code",
            table: "AdministrativeAreas",
            columns: new[] { "CountryAdministrativeLevelId", "ParentId", "Code" },
            unique: true,
            filter: "[ParentId] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_AdministrativeAreas_ParentId",
            table: "AdministrativeAreas",
            column: "ParentId");

        migrationBuilder.CreateIndex(
            name: "IX_AdministrativeAreaTypes_CountryAdministrativeLevelId_Code",
            table: "AdministrativeAreaTypes",
            columns: new[] { "CountryAdministrativeLevelId", "Code" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Countries_Code",
            table: "Countries",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_CountryAdministrativeLevels_CountryId_Level_DisplayName",
            table: "CountryAdministrativeLevels",
            columns: new[] { "CountryId", "Level", "DisplayName" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_LocationSearches_AdministrativeAreaId",
            table: "LocationSearches",
            column: "AdministrativeAreaId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_LocationSearches_SearchText",
            table: "LocationSearches",
            column: "SearchText");

        migrationBuilder.AddForeignKey(
            name: "FK_Branches_LocationSearches_LocationSearchId",
            table: "Branches",
            column: "LocationSearchId",
            principalTable: "LocationSearches",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.DropColumn(
            name: "City",
            table: "Branches");

        migrationBuilder.Sql(ReadEmbeddedSql(
            "BarberFlow.Infrastructure.Catalogs.Nicaragua.001_SeedCountryAndLevels.sql"));
        migrationBuilder.Sql(ReadEmbeddedSql(
            "BarberFlow.Infrastructure.Catalogs.Nicaragua.002_SeedNicaraguaTerritorialCatalog.sql"),
            suppressTransaction: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "City",
            table: "Branches",
            type: "nvarchar(120)",
            maxLength: 120,
            nullable: false,
            defaultValue: "");

        migrationBuilder.DropForeignKey(
            name: "FK_Branches_LocationSearches_LocationSearchId",
            table: "Branches");

        migrationBuilder.DropTable(
            name: "LocationSearches");

        migrationBuilder.DropTable(
            name: "AdministrativeAreas");

        migrationBuilder.DropTable(
            name: "AdministrativeAreaTypes");

        migrationBuilder.DropTable(
            name: "CountryAdministrativeLevels");

        migrationBuilder.DropTable(
            name: "Countries");

        migrationBuilder.DropIndex(
            name: "IX_Branches_LocationSearchId",
            table: "Branches");

        migrationBuilder.DropColumn(
            name: "LocationSearchId",
            table: "Branches");
    }

    private static string ReadEmbeddedSql(string resourceName)
    {
        var assembly = typeof(AddTerritorialCatalog).Assembly;
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded catalog resource not found: {resourceName}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
