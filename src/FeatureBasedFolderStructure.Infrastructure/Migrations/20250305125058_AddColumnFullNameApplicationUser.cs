using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureBasedFolderStructure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnFullNameApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_application_users_user_name",
                table: "application_users");

            migrationBuilder.DropColumn(
                name: "user_name",
                table: "application_users");

            migrationBuilder.AddColumn<string>(
                name: "full_name",
                table: "application_users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "full_name",
                table: "application_users");

            migrationBuilder.AddColumn<string>(
                name: "user_name",
                table: "application_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_application_users_user_name",
                table: "application_users",
                column: "user_name",
                unique: true);
        }
    }
}
