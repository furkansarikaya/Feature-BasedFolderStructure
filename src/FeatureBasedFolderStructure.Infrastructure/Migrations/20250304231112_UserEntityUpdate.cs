using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureBasedFolderStructure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserEntityUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "access_failed_count",
                table: "application_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "email_confirmed",
                table: "application_users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "lockout_end",
                table: "application_users",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "access_failed_count",
                table: "application_users");

            migrationBuilder.DropColumn(
                name: "email_confirmed",
                table: "application_users");

            migrationBuilder.DropColumn(
                name: "lockout_end",
                table: "application_users");
        }
    }
}
