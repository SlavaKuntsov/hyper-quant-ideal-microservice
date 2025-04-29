using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoSymbolServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSymbolUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_symbols_exchange_id_symbol_name",
                table: "symbols");

            migrationBuilder.CreateIndex(
                name: "ix_symbols_exchange_id_symbol_name_market_type",
                table: "symbols",
                columns: new[] { "exchange_id", "symbol_name", "market_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_symbols_exchange_id_symbol_name_market_type",
                table: "symbols");

            migrationBuilder.CreateIndex(
                name: "ix_symbols_exchange_id_symbol_name",
                table: "symbols",
                columns: new[] { "exchange_id", "symbol_name" });
        }
    }
}
