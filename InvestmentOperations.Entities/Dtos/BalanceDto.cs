using InvestmentOperations.Core.Entities;

namespace InvestmentOperations.Entities.Dtos
{
    public class BalanceDto : IDto
    {
        public int BalanceId { get; set; }
        public int UserId { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetCode { get; set; }
        public string AssetType { get; set; }
        public decimal Amount { get; set; }
    }
}
