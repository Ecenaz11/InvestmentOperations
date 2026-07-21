using InvestmentOperations.Core.Entities;

namespace InvestmentOperations.Entities.Dtos
{
    public class TradeForUpdateDto : IDto
    {
        public int TradeId { get; set; }
        public int AssetId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string TradeType { get; set; }
        public decimal UnitPrice { get; set; }

    }
}