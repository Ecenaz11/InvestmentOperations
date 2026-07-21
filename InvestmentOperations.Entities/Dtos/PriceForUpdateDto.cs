using InvestmentOperations.Core.Entities;

namespace InvestmentOperations.Entities.Dtos
{
    public class PriceForUpdateDto : IDto
    {
        public int PriceId { get; set; }
        public int AssetId { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}