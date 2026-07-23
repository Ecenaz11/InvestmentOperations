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
            // Not: users_email_key, trades_ind_01, assets_assetcode_key, fk_prices_assets,
            // fk_trades_assets, fk_trades_users zaten veritabanında elle eklenmişti (bkz. proje notları).
            // Bu migration sadece EF modelini onlarla eşleştiriyor, gerçekten yeni olan iki işlemi uyguluyor:
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
