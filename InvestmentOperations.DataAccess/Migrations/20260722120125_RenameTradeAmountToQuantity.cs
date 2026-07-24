using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestmentOperations.DataAccess.Migrations
{
    
    public partial class RenameTradeAmountToQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "amount",
                table: "trades",
                newName: "quantity");
        }

       
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "trades",
                newName: "amount");
        }
    }
}
