using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InvestmentOperations.DataAccess.Migrations
{
    
    public partial class AddBalanceTable : Migration
    {
       
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "balances",
                columns: table => new
                {
                    balanceid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    assetid = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    updatedat = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_balances", x => x.balanceid);
                });
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "balances");

        }
    }
}
