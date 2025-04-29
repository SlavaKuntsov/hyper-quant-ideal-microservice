using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoSymbolServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSymbol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_symbols_exchange_id_pair",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "close_time",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "expiration_date",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "funding_rate",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "is_margin",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "last_update",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "max_leverage",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "max_price",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "max_size",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "min_price",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "min_size",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "open_time",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "price_step",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "should_syncronize",
                table: "symbols");

            migrationBuilder.RenameColumn(
                name: "trade_start",
                table: "symbols",
                newName: "delivery_date");

            migrationBuilder.RenameColumn(
                name: "symbol_type",
                table: "symbols",
                newName: "symbol_name");

            migrationBuilder.RenameColumn(
                name: "pair",
                table: "symbols",
                newName: "quote_asset");

            migrationBuilder.RenameColumn(
                name: "match_symbol",
                table: "symbols",
                newName: "market_type");

            migrationBuilder.RenameColumn(
                name: "board",
                table: "symbols",
                newName: "base_asset");

            migrationBuilder.AddColumn<string>(
                name: "contract_type",
                table: "symbols",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "margin_asset",
                table: "symbols",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "max_quantity",
                table: "symbols",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "min_notional",
                table: "symbols",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "min_quantity",
                table: "symbols",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "price_precision",
                table: "symbols",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "quantity_precision",
                table: "symbols",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "should_synchronize",
                table: "symbols",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "ix_symbols_exchange_id_symbol_name",
                table: "symbols",
                columns: new[] { "exchange_id", "symbol_name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_symbols_exchange_id_symbol_name",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "contract_type",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "margin_asset",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "max_quantity",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "min_notional",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "min_quantity",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "price_precision",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "quantity_precision",
                table: "symbols");

            migrationBuilder.DropColumn(
                name: "should_synchronize",
                table: "symbols");

            migrationBuilder.RenameColumn(
                name: "symbol_name",
                table: "symbols",
                newName: "symbol_type");

            migrationBuilder.RenameColumn(
                name: "quote_asset",
                table: "symbols",
                newName: "pair");

            migrationBuilder.RenameColumn(
                name: "market_type",
                table: "symbols",
                newName: "match_symbol");

            migrationBuilder.RenameColumn(
                name: "delivery_date",
                table: "symbols",
                newName: "trade_start");

            migrationBuilder.RenameColumn(
                name: "base_asset",
                table: "symbols",
                newName: "board");

            migrationBuilder.AddColumn<DateTime>(
                name: "close_time",
                table: "symbols",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "expiration_date",
                table: "symbols",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "funding_rate",
                table: "symbols",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_margin",
                table: "symbols",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_update",
                table: "symbols",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "max_leverage",
                table: "symbols",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "max_price",
                table: "symbols",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "max_size",
                table: "symbols",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "min_price",
                table: "symbols",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "min_size",
                table: "symbols",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "open_time",
                table: "symbols",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "price_step",
                table: "symbols",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "should_syncronize",
                table: "symbols",
                type: "boolean",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_symbols_exchange_id_pair",
                table: "symbols",
                columns: new[] { "exchange_id", "pair" });
        }
    }
}
