using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoSymbolServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_enabled",
                table: "notification_settings",
                newName: "is_telegram_enabled");

            migrationBuilder.AddColumn<bool>(
                name: "is_email_enabled",
                table: "notification_settings",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "notification_settings",
                keyColumn: "id",
                keyValue: new Guid("5a23149e-79cc-4fed-8533-c3b4415c2cdb"),
                column: "is_email_enabled",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_email_enabled",
                table: "notification_settings");

            migrationBuilder.RenameColumn(
                name: "is_telegram_enabled",
                table: "notification_settings",
                newName: "is_enabled");
        }
    }
}
