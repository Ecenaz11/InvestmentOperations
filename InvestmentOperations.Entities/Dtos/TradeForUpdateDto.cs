using InvestmentOperations.Core.Entities;
using InvestmentOperations.Entities.Enums;

namespace InvestmentOperations.Entities.Dtos
{
    public class TradeForUpdateDto : IDto
    {
        public int TradeId { get; set; }
        public int AssetId { get; set; }
        public int UserId { get; set; }
        public decimal Quantity { get; set; }
        public TradeType TradeType { get; set; }
    }
}