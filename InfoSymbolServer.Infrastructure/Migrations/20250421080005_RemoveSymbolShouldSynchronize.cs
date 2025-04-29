using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoSymbolServer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSymbolShouldSynchronize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "should_synchronize",
                table: "symbols");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "should_synchronize",
                table: "symbols",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }
    }
}
