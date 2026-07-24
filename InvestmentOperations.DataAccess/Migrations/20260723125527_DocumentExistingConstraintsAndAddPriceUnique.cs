using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentOperations.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DocumentExistingConstraintsAndAddPriceUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateIndex(
                name: "IX_trades_assetid",
                table: "trades",
                column: "assetid");

            migrationBuilder.CreateIndex(
                name: "IX_prices_assetid",
                table: "prices",
                column: "assetid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_trades_assetid",
                table: "trades");

            migrationBuilder.DropIndex(
                name: "IX_prices_assetid",
                table: "prices");
        }
    }
}
