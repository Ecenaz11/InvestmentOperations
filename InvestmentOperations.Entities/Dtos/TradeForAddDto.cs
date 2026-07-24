using InvestmentOperations.Core.Entities;
using InvestmentOperations.Entities.Enums;

namespace InvestmentOperations.Entities.Dtos
{
    public class TradeForAddDto : IDto
    {
        public int AssetId { get; set; }
        public int UserId { get; set; }
        public decimal Quantity { get; set; }
        public TradeType TradeType { get; set; }
    }
}