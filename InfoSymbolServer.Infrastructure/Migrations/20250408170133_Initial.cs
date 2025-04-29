using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoSymbolServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "exchanges",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_spot = table.Column<bool>(type: "boolean", nullable: false),
                    is_futures = table.Column<bool>(type: "boolean", nullable: false),
                    is_options = table.Column<bool>(type: "boolean", nullable: false),
                    order_types = table.Column<List<string>>(type: "TEXT[]", nullable: false),
                    min_trade_size = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exchanges", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "symbols",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    exchange_id = table.Column<Guid>(type: "uuid", nullable: false),
                    pair = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    symbol_type = table.Column<string>(type: "text", nullable: false),
                    is_margin = table.Column<bool>(type: "boolean", nullable: false),
                    price_step = table.Column<decimal>(type: "numeric", nullable: true),
                    board = table.Column<string>(type: "text", nullable: false),
                    max_leverage = table.Column<decimal>(type: "numeric", nullable: true),
                    trade_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    funding_rate = table.Column<decimal>(type: "numeric", nullable: true),
                    match_symbol = table.Column<string>(type: "text", nullable: false),
                    min_price = table.Column<decimal>(type: "numeric", nullable: true),
                    max_price = table.Column<decimal>(type: "numeric", nullable: true),
                    min_size = table.Column<decimal>(type: "numeric", nullable: true),
                    max_size = table.Column<decimal>(type: "numeric", nullable: true),
                    last_update = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    should_syncronize = table.Column<bool>(type: "boolean", nullable: true),
                    open_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    close_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_symbols", x => x.id);
                    table.ForeignKey(
                        name: "fk_symbols_exchanges_exchange_id",
                        column: x => x.exchange_id,
                        principalTable: "exchanges",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_symbols_exchange_id_pair",
                table: "symbols",
                columns: new[] { "exchange_id", "pair" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "symbols");

            migrationBuilder.DropTable(
                name: "exchanges");
        }
    }
}
