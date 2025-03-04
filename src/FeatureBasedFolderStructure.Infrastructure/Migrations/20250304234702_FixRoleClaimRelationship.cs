using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureBasedFolderStructure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRoleClaimRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_role_claims_roles_role_id1",
                table: "role_claims");

            migrationBuilder.DropIndex(
                name: "ix_role_claims_role_id1",
                table: "role_claims");

            migrationBuilder.DropColumn(
                name: "role_id1",
                table: "role_claims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "role_id1",
                table: "role_claims",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_role_claims_role_id1",
                table: "role_claims",
                column: "role_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_role_claims_roles_role_id1",
                table: "role_claims",
                column: "role_id1",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
