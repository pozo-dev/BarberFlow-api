using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LinkCollaboratorsToUserProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collaborators_Roles_RoleId",
                table: "Collaborators");

            migrationBuilder.DropIndex(
                name: "IX_Collaborators_RoleId",
                table: "Collaborators");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Collaborators");

            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "Collaborators",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Collaborators_UserProfileId",
                table: "Collaborators",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collaborators_UserProfiles_UserProfileId",
                table: "Collaborators",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collaborators_UserProfiles_UserProfileId",
                table: "Collaborators");

            migrationBuilder.DropIndex(
                name: "IX_Collaborators_UserProfileId",
                table: "Collaborators");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Collaborators");

            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "Collaborators",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Collaborators_RoleId",
                table: "Collaborators",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collaborators_Roles_RoleId",
                table: "Collaborators",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
