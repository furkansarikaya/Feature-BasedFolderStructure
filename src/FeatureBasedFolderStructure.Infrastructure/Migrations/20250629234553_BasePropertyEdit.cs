using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeatureBasedFolderStructure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BasePropertyEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_tokens_user_id_token_type_token_value_is_deleted",
                table: "user_tokens");

            migrationBuilder.DropIndex(
                name: "ix_user_roles_user_id_role_id_is_deleted",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "user_tokens");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "user_tokens");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "user_tokens");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "roles");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "role_claims");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "role_claims");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "role_claims");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "order_items",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deleted_by",
                table: "order_items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "order_items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_user_tokens_user_id_token_type_token_value",
                table: "user_tokens",
                columns: new[] { "user_id", "token_type", "token_value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id_role_id",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_user_tokens_user_id_token_type_token_value",
                table: "user_tokens");

            migrationBuilder.DropIndex(
                name: "ix_user_roles_user_id_role_id",
                table: "user_roles");

            migrationBuilder.DropColumn(
                name: "deleted_at",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "deleted_by",
                table: "order_items");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "order_items");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "user_tokens",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deleted_by",
                table: "user_tokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "user_tokens",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "user_roles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deleted_by",
                table: "user_roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "user_roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "roles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deleted_by",
                table: "roles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "roles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_at",
                table: "role_claims",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "deleted_by",
                table: "role_claims",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "role_claims",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "ix_user_tokens_user_id_token_type_token_value_is_deleted",
                table: "user_tokens",
                columns: new[] { "user_id", "token_type", "token_value", "is_deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id_role_id_is_deleted",
                table: "user_roles",
                columns: new[] { "user_id", "role_id", "is_deleted" },
                unique: true);
        }
    }
}
