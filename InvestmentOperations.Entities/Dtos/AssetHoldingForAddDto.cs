using InvestmentOperations.Entities.Dtos;
namespace InvestmentOperations.Entities.Dtos
{
    public class AssetHoldingForAddDto
    {
        public int UserId { get; set; }
        public int AssetId { get; set; }
        public decimal Amount { get; set; }
    }
}