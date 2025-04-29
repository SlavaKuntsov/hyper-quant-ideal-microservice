using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoSymbolServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_futures",
                table: "exchanges");

            migrationBuilder.DropColumn(
                name: "is_options",
                table: "exchanges");

            migrationBuilder.DropColumn(
                name: "is_spot",
                table: "exchanges");

            migrationBuilder.DropColumn(
                name: "min_trade_size",
                table: "exchanges");

            migrationBuilder.DropColumn(
                name: "order_types",
                table: "exchanges");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "symbols",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "exchanges",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "ix_exchanges_name",
                table: "exchanges",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_exchanges_name",
                table: "exchanges");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "exchanges");

            migrationBuilder.AddColumn<bool>(
                name: "is_futures",
                table: "exchanges",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_options",
                table: "exchanges",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_spot",
                table: "exchanges",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "min_trade_size",
                table: "exchanges",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "order_types",
                table: "exchanges",
                type: "TEXT[]",
                nullable: false);
        }
    }
}
