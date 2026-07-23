using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentOperations.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBalanceConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_balances_assetid",
                table: "balances",
                column: "assetid");

            migrationBuilder.CreateIndex(
                name: "IX_balances_userid_assetid",
                table: "balances",
                columns: new[] { "userid", "assetid" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_balances_assets_assetid",
                table: "balances",
                column: "assetid",
                principalTable: "assets",
                principalColumn: "assetid");

            migrationBuilder.AddForeignKey(
                name: "FK_balances_users_userid",
                table: "balances",
                column: "userid",
                principalTable: "users",
                principalColumn: "userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_balances_assets_assetid",
                table: "balances");

            migrationBuilder.DropForeignKey(
                name: "FK_balances_users_userid",
                table: "balances");

            migrationBuilder.DropIndex(
                name: "IX_balances_assetid",
                table: "balances");

            migrationBuilder.DropIndex(
                name: "IX_balances_userid_assetid",
                table: "balances");
        }
    }
}
