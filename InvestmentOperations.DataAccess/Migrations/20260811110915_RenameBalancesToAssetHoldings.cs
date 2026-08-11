using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InvestmentOperations.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RenameBalancesToAssetHoldings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "balances",
                newName: "assetholdings");

            migrationBuilder.RenameColumn(
                name: "balanceid",
                table: "assetholdings",
                newName: "assetholdingid");

            migrationBuilder.Sql(
                "ALTER TABLE assetholdings RENAME CONSTRAINT \"PK_balances\" TO \"PK_assetholdings\";");

            migrationBuilder.Sql(
                "ALTER TABLE assetholdings RENAME CONSTRAINT \"FK_balances_assets_assetid\" TO \"FK_assetholdings_assets_assetid\";");

            migrationBuilder.Sql(
                "ALTER TABLE assetholdings RENAME CONSTRAINT \"FK_balances_users_userid\" TO \"FK_assetholdings_users_userid\";");

            migrationBuilder.RenameIndex(
                name: "IX_balances_assetid",
                table: "assetholdings",
                newName: "IX_assetholdings_assetid");

            migrationBuilder.RenameIndex(
                name: "IX_balances_userid_assetid",
                table: "assetholdings",
                newName: "IX_assetholdings_userid_assetid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_assetholdings_userid_assetid",
                table: "assetholdings",
                newName: "IX_balances_userid_assetid");

            migrationBuilder.RenameIndex(
                name: "IX_assetholdings_assetid",
                table: "assetholdings",
                newName: "IX_balances_assetid");

            migrationBuilder.Sql(
                "ALTER TABLE assetholdings RENAME CONSTRAINT \"FK_assetholdings_users_userid\" TO \"FK_balances_users_userid\";");

            migrationBuilder.Sql(
                "ALTER TABLE assetholdings RENAME CONSTRAINT \"FK_assetholdings_assets_assetid\" TO \"FK_balances_assets_assetid\";");

            migrationBuilder.Sql(
                "ALTER TABLE assetholdings RENAME CONSTRAINT \"PK_assetholdings\" TO \"PK_balances\";");

            migrationBuilder.RenameColumn(
                name: "assetholdingid",
                table: "assetholdings",
                newName: "balanceid");

            migrationBuilder.RenameTable(
                name: "assetholdings",
                newName: "balances");
        }
    }
}
