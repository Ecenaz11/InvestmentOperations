using InvestmentOperations.Core.Entities;

namespace InvestmentOperations.Entities.Dtos
{
    public class PriceForAddDto : IDto
    {
        public int AssetId { get; set; }
        public decimal CurrentPrice { get; set; }
    }
}

    
   