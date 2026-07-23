using InvestmentOperations.Core.Entities;

namespace InvestmentOperations.Entities.Dtos
{
    public class TradeForAddDto : IDto
    {
        public int AssetId { get; set; }
        public int UserId { get; set; }
        public decimal Quantity { get; set; }
        public string TradeType { get; set; }
    }
}